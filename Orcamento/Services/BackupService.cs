namespace Orcamento.Services;

/// <summary>
/// Serviço de backup automático do banco de dados
/// </summary>
public class BackupService
{
    private readonly LoggingService _logger;
    private readonly NotificationService _notificationService;
    private readonly IPreferences _preferences;
    private readonly string _dbPath;
    private readonly string _backupDirectory;

    public BackupService(
        LoggingService logger,
        NotificationService notificationService,
        IPreferences preferences)
    {
        _logger = logger;
        _notificationService = notificationService;
        _preferences = preferences;

        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "Orcamento.db3");
        _backupDirectory = Path.Combine(FileSystem.AppDataDirectory, "backups");

        // Criar diretório de backups
        Directory.CreateDirectory(_backupDirectory);
    }

    /// <summary>
    /// Cria backup do banco de dados
    /// </summary>
    public async Task<string> CreateBackupAsync()
    {
        await _logger.LogInfoAsync("Iniciando backup do banco de dados", "BackupService");

        try
        {
            if (!File.Exists(_dbPath))
            {
                throw new FileNotFoundException("Banco de dados não encontrado", _dbPath);
            }

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"backup_{timestamp}.db3";
            var backupPath = Path.Combine(_backupDirectory, backupFileName);

            // Copiar arquivo
            File.Copy(_dbPath, backupPath, overwrite: true);

            // Atualizar última data de backup
            _preferences.Set("last_backup_date", DateTime.Now.ToString("O"));

            // Limpar backups antigos (manter últimos 7)
            await CleanOldBackupsAsync();

            await _logger.LogInfoAsync($"Backup criado com sucesso: {backupFileName}", "BackupService");

            return backupPath;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync("Erro ao criar backup", ex, "BackupService");
            throw;
        }
    }

    /// <summary>
    /// Restaura banco de dados de um backup
    /// </summary>
    public async Task<bool> RestoreBackupAsync(string backupPath)
    {
        await _logger.LogInfoAsync($"Restaurando backup: {backupPath}", "BackupService");

        try
        {
            if (!File.Exists(backupPath))
            {
                throw new FileNotFoundException("Backup não encontrado", backupPath);
            }

            // Criar backup de segurança antes de restaurar
            var safetyBackup = await CreateBackupAsync();

            try
            {
                // Restaurar
                File.Copy(backupPath, _dbPath, overwrite: true);
                
                await _logger.LogInfoAsync("Backup restaurado com sucesso", "BackupService");
                return true;
            }
            catch
            {
                // Se falhar, restaurar o backup de segurança
                File.Copy(safetyBackup, _dbPath, overwrite: true);
                throw;
            }
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync("Erro ao restaurar backup", ex, "BackupService");
            return false;
        }
    }

    /// <summary>
    /// Lista todos os backups disponíveis
    /// </summary>
    public List<BackupInfo> ListBackups()
    {
        var backups = new List<BackupInfo>();

        var files = Directory.GetFiles(_backupDirectory, "backup_*.db3")
            .OrderByDescending(f => new FileInfo(f).CreationTime);

        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            backups.Add(new BackupInfo
            {
                FilePath = file,
                FileName = fileInfo.Name,
                CreatedAt = fileInfo.CreationTime,
                Size = fileInfo.Length,
                SizeFormatted = FormatFileSize(fileInfo.Length)
            });
        }

        return backups;
    }

    /// <summary>
    /// Verifica se é necessário fazer backup
    /// </summary>
    public bool NeedsBackup(int daysThreshold = 7)
    {
        var lastBackupStr = _preferences.Get("last_backup_date", string.Empty);
        
        if (string.IsNullOrEmpty(lastBackupStr))
            return true;

        if (DateTime.TryParse(lastBackupStr, out var lastBackup))
        {
            return (DateTime.Now - lastBackup).TotalDays >= daysThreshold;
        }

        return true;
    }

    /// <summary>
    /// Backup automático agendado
    /// </summary>
    public async Task ScheduleAutomaticBackupAsync()
    {
        if (NeedsBackup())
        {
            try
            {
                await CreateBackupAsync();
                await _notificationService.NotifyBackupRecommendedAsync();
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Erro no backup automático", ex, "BackupService");
            }
        }
    }

    /// <summary>
    /// Exporta backup para compartilhar
    /// </summary>
    public async Task<string> ExportBackupAsync()
    {
        var backupPath = await CreateBackupAsync();
        var exportPath = Path.Combine(FileSystem.CacheDirectory, Path.GetFileName(backupPath));
        
        File.Copy(backupPath, exportPath, overwrite: true);
        
        return exportPath;
    }

    /// <summary>
    /// Importa backup de arquivo externo
    /// </summary>
    public async Task<bool> ImportBackupAsync(string externalBackupPath)
    {
        try
        {
            if (!File.Exists(externalBackupPath))
                return false;

            var backupFileName = $"imported_{DateTime.Now:yyyyMMdd_HHmmss}.db3";
            var backupPath = Path.Combine(_backupDirectory, backupFileName);

            // Copiar para diretório de backups
            File.Copy(externalBackupPath, backupPath, overwrite: true);

            // Restaurar
            return await RestoreBackupAsync(backupPath);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync("Erro ao importar backup", ex, "BackupService");
            return false;
        }
    }

    /// <summary>
    /// Remove backups antigos (mantém últimos N)
    /// </summary>
    private async Task CleanOldBackupsAsync(int keepLast = 7)
    {
        try
        {
            var files = Directory.GetFiles(_backupDirectory, "backup_*.db3")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .Skip(keepLast);

            foreach (var file in files)
            {
                file.Delete();
                await _logger.LogInfoAsync($"Backup antigo removido: {file.Name}", "BackupService");
            }
        }
        catch (Exception ex)
        {
            await _logger.LogWarningAsync($"Erro ao limpar backups antigos: {ex.Message}", "BackupService");
        }
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Informações sobre um backup
/// </summary>
public class BackupInfo
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long Size { get; set; }
    public string SizeFormatted { get; set; } = string.Empty;
    public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy HH:mm");
}
