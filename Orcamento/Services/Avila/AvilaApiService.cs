using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Orcamento.Services.Avila;

/// <summary>
/// Serviço de integração com api.avila.inc
/// Gerencia autenticação, sincronização e comunicação com o backend central
/// </summary>
public class AvilaApiService
{
    private readonly HttpClient _httpClient;
    private readonly ISecureStorage _secureStorage;
    private readonly IConnectivity _connectivity;
    private string? _accessToken;
    private string? _refreshToken;

    // URLs por ambiente
#if DEBUG
    private const string BaseUrl = "https://localhost:7000";
#elif STAGING
    private const string BaseUrl = "https://api-staging.avila.inc";
#else
    private const string BaseUrl = "https://api.avila.inc";
#endif

    public AvilaApiService(
        HttpClient httpClient,
        ISecureStorage secureStorage,
        IConnectivity connectivity)
    {
        _httpClient = httpClient;
        _secureStorage = secureStorage;
        _connectivity = connectivity;

        _httpClient.BaseAddress = new Uri(BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-Client-App", "Orcamento");
        _httpClient.DefaultRequestHeaders.Add("X-Client-Version", AppInfo.VersionString);
        _httpClient.DefaultRequestHeaders.Add("X-Client-Platform", DeviceInfo.Platform.ToString());
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    #region Autenticação

    /// <summary>
    /// Autentica usuário via email/senha
    /// </summary>
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return AuthResult.Failed("Sem conexão com a internet");

            var response = await _httpClient.PostAsJsonAsync("/auth/login", new
            {
                email,
                password,
                clientId = "Orcamento",
                platform = DeviceInfo.Platform.ToString(),
                deviceId = await GetDeviceIdAsync()
            });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthTokenResponse>();
                if (result != null)
                {
                    await SaveTokensAsync(result.AccessToken, result.RefreshToken);
                    return AuthResult.Success(result.User);
                }
            }

            var error = await response.Content.ReadAsStringAsync();
            return AuthResult.Failed(error);
        }
        catch (Exception ex)
        {
            await LogErrorAsync("Login", ex);
            return AuthResult.Failed($"Erro ao fazer login: {ex.Message}");
        }
    }

    /// <summary>
    /// Registra novo usuário
    /// </summary>
    public async Task<AuthResult> RegisterAsync(string name, string email, string password)
    {
        try
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return AuthResult.Failed("Sem conexão com a internet");

            var response = await _httpClient.PostAsJsonAsync("/auth/register", new
            {
                name,
                email,
                password,
                clientId = "Orcamento",
                platform = DeviceInfo.Platform.ToString(),
                deviceId = await GetDeviceIdAsync()
            });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthTokenResponse>();
                if (result != null)
                {
                    await SaveTokensAsync(result.AccessToken, result.RefreshToken);
                    return AuthResult.Success(result.User);
                }
            }

            var error = await response.Content.ReadAsStringAsync();
            return AuthResult.Failed(error);
        }
        catch (Exception ex)
        {
            await LogErrorAsync("Register", ex);
            return AuthResult.Failed($"Erro ao registrar: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza access token usando refresh token
    /// </summary>
    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await _secureStorage.GetAsync("refresh_token");
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var response = await _httpClient.PostAsJsonAsync("/auth/refresh", new
            {
                refreshToken,
                clientId = "Orcamento"
            });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthTokenResponse>();
                if (result != null)
                {
                    await SaveTokensAsync(result.AccessToken, result.RefreshToken);
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Faz logout do usuário
    /// </summary>
    public async Task LogoutAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();

            await _httpClient.PostAsync("/auth/logout", null);
        }
        catch
        {
            // Ignora erros no logout
        }
        finally
        {
            await ClearTokensAsync();
        }
    }

    /// <summary>
    /// Valida se o token atual é válido
    /// </summary>
    public async Task<bool> ValidateTokenAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var response = await _httpClient.GetAsync("/auth/validate");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Sincronização

    /// <summary>
    /// Sincroniza dados locais com a nuvem
    /// </summary>
    public async Task<SyncResult> SyncDataAsync(SyncPayload payload)
    {
        try
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return SyncResult.Failed("Sem conexão com a internet");

            await EnsureAuthenticatedAsync();

            var response = await _httpClient.PostAsJsonAsync("/sync/upload", payload);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<SyncResponse>();
                if (result != null)
                {
                    return SyncResult.Success(result);
                }
            }

            return SyncResult.Failed("Erro ao sincronizar dados");
        }
        catch (Exception ex)
        {
            await LogErrorAsync("Sync", ex);
            return SyncResult.Failed($"Erro na sincronização: {ex.Message}");
        }
    }

    /// <summary>
    /// Baixa dados atualizados da nuvem
    /// </summary>
    public async Task<DownloadResult> DownloadDataAsync(DateTime? lastSync = null)
    {
        try
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return DownloadResult.Failed("Sem conexão com a internet");

            await EnsureAuthenticatedAsync();

            var url = lastSync.HasValue
                ? $"/sync/download?since={lastSync.Value:O}"
                : "/sync/download";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<DownloadResponse>();
                if (result != null)
                {
                    return DownloadResult.Success(result);
                }
            }

            return DownloadResult.Failed("Erro ao baixar dados");
        }
        catch (Exception ex)
        {
            await LogErrorAsync("Download", ex);
            return DownloadResult.Failed($"Erro ao baixar dados: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém status da sincronização
    /// </summary>
    public async Task<SyncStatus?> GetSyncStatusAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var response = await _httpClient.GetAsync("/sync/status");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SyncStatus>();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Resolve conflitos de sincronização
    /// </summary>
    public async Task<bool> ResolveConflictsAsync(List<ConflictResolution> resolutions)
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var response = await _httpClient.PostAsJsonAsync("/sync/resolve-conflicts", resolutions);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Analytics

    /// <summary>
    /// Registra evento de analytics
    /// </summary>
    public async Task TrackEventAsync(string eventName, Dictionary<string, object>? properties = null)
    {
        try
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return;

            await _httpClient.PostAsJsonAsync("/analytics/events", new
            {
                eventName,
                properties,
                timestamp = DateTime.UtcNow,
                userId = await GetUserIdAsync(),
                deviceId = await GetDeviceIdAsync(),
                platform = DeviceInfo.Platform.ToString(),
                appVersion = AppInfo.VersionString
            });
        }
        catch
        {
            // Silenciosamente ignora erros de analytics
        }
    }

    /// <summary>
    /// Registra erro para monitoramento
    /// </summary>
    public async Task LogErrorAsync(string context, Exception exception)
    {
        try
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return;

            await _httpClient.PostAsJsonAsync("/analytics/errors", new
            {
                context,
                message = exception.Message,
                stackTrace = exception.StackTrace,
                timestamp = DateTime.UtcNow,
                userId = await GetUserIdAsync(),
                deviceId = await GetDeviceIdAsync(),
                platform = DeviceInfo.Platform.ToString(),
                appVersion = AppInfo.VersionString
            });
        }
        catch
        {
            // Silenciosamente ignora erros de logging
        }
    }

    /// <summary>
    /// Obtém insights de dados
    /// </summary>
    public async Task<AnalyticsInsights?> GetInsightsAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var response = await _httpClient.GetAsync("/analytics/insights");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AnalyticsInsights>();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region User Management

    /// <summary>
    /// Obtém perfil do usuário
    /// </summary>
    public async Task<UserProfile?> GetProfileAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var response = await _httpClient.GetAsync("/users/profile");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserProfile>();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Atualiza perfil do usuário
    /// </summary>
    public async Task<bool> UpdateProfileAsync(UserProfile profile)
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var response = await _httpClient.PutAsJsonAsync("/users/profile", profile);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtém permissões do usuário
    /// </summary>
    public async Task<UserPermissions?> GetPermissionsAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var response = await _httpClient.GetAsync("/users/permissions");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserPermissions>();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region Helpers Privados

    private async Task EnsureAuthenticatedAsync()
    {
        if (_accessToken == null)
        {
            _accessToken = await _secureStorage.GetAsync("access_token");
        }

        if (string.IsNullOrEmpty(_accessToken))
        {
            throw new UnauthorizedAccessException("Usuário não autenticado");
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    private async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        _accessToken = accessToken;
        _refreshToken = refreshToken;

        await _secureStorage.SetAsync("access_token", accessToken);
        await _secureStorage.SetAsync("refresh_token", refreshToken);
    }

    private async Task ClearTokensAsync()
    {
        _accessToken = null;
        _refreshToken = null;

        _secureStorage.Remove("access_token");
        _secureStorage.Remove("refresh_token");
    }

    private async Task<string> GetDeviceIdAsync()
    {
        var deviceId = await _secureStorage.GetAsync("device_id");

        if (string.IsNullOrEmpty(deviceId))
        {
            deviceId = Guid.NewGuid().ToString();
            await _secureStorage.SetAsync("device_id", deviceId);
        }

        return deviceId;
    }

    private async Task<string?> GetUserIdAsync()
    {
        return await _secureStorage.GetAsync("user_id");
    }

    #endregion
}

#region DTOs e Modelos

public record AuthTokenResponse(string AccessToken, string RefreshToken, UserInfo User);
public record UserInfo(string Id, string Name, string Email, string Plan);

public class AuthResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public UserInfo? User { get; init; }

    public static AuthResult Success(UserInfo user) => new() { IsSuccess = true, User = user };
    public static AuthResult Failed(string error) => new() { IsSuccess = false, ErrorMessage = error };
}

public class SyncPayload
{
    public List<object>? CreatedItems { get; set; }
    public List<object>? UpdatedItems { get; set; }
    public List<string>? DeletedItems { get; set; }
    public DateTime LastSyncAt { get; set; }
}

public class SyncResponse
{
    public bool Success { get; set; }
    public int ItemsSynced { get; set; }
    public List<ConflictInfo>? Conflicts { get; set; }
    public DateTime SyncedAt { get; set; }
}

public class SyncResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public SyncResponse? Data { get; init; }

    public static SyncResult Success(SyncResponse data) => new() { IsSuccess = true, Data = data };
    public static SyncResult Failed(string error) => new() { IsSuccess = false, ErrorMessage = error };
}

public class DownloadResponse
{
    public List<object>? Items { get; set; }
    public DateTime ServerTimestamp { get; set; }
}

public class DownloadResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public DownloadResponse? Data { get; init; }

    public static DownloadResult Success(DownloadResponse data) => new() { IsSuccess = true, Data = data };
    public static DownloadResult Failed(string error) => new() { IsSuccess = false, ErrorMessage = error };
}

public class SyncStatus
{
    public DateTime? LastSync { get; set; }
    public bool IsSyncing { get; set; }
    public int PendingItems { get; set; }
}

public class ConflictInfo
{
    public string ItemId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public object LocalVersion { get; set; } = new();
    public object ServerVersion { get; set; } = new();
}

public class ConflictResolution
{
    public string ItemId { get; set; } = string.Empty;
    public string Resolution { get; set; } = "server-wins"; // "server-wins", "client-wins", "merge"
}

public class AnalyticsInsights
{
    public decimal AverageMonthlyExpense { get; set; }
    public string? TopCategory { get; set; }
    public List<string>? Recommendations { get; set; }
}

public class UserProfile
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public string Plan { get; set; } = "free";
}

public class UserPermissions
{
    public bool CanSync { get; set; }
    public bool CanExport { get; set; }
    public bool CanUseOpenBanking { get; set; }
    public bool CanUseAI { get; set; }
    public int MaxAccounts { get; set; }
}

#endregion
