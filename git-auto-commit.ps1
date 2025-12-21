# ============================================================
# 🚀 GIT AUTO COMMIT - PowerShell Edition
# ============================================================
# Desenvolvido por: Nícolas Ávila
# Usa credenciais das variáveis de ambiente (MCP)
# ============================================================

[CmdletBinding()]
param(
    [string]$CommitMessage = "",
    [switch]$Force = $false,
    [switch]$Verbose = $false
)

# Configuração
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

# Cores
function Write-ColorOutput($ForegroundColor) {
    $fc = $host.UI.RawUI.ForegroundColor
    $host.UI.RawUI.ForegroundColor = $ForegroundColor
    if ($args) {
        Write-Output $args
    }
    $host.UI.RawUI.ForegroundColor = $fc
}

function Write-Header {
    param([string]$Text)
    Write-Host ""
    Write-ColorOutput Yellow "============================================================"
    Write-ColorOutput Yellow "  $Text"
    Write-ColorOutput Yellow "============================================================"
    Write-Host ""
}

function Write-Step {
    param([string]$Step, [string]$Text)
    Write-ColorOutput Cyan "[$Step] $Text"
}

function Write-Success {
    param([string]$Text)
    Write-ColorOutput Green "✅ $Text"
}

function Write-Error2 {
    param([string]$Text)
    Write-ColorOutput Red "❌ $Text"
}

function Write-Warning2 {
    param([string]$Text)
    Write-ColorOutput Yellow "⚠️  $Text"
}

function Write-Info {
    param([string]$Text)
    Write-ColorOutput Gray "ℹ️  $Text"
}

# ============================================================
# INICIALIZAÇÃO
# ============================================================

Write-Header "🚀 GIT AUTO COMMIT - Orçamento Familiar"

Write-Info "Desenvolvido por: Nícolas Ávila"
Write-Info "Repositório: https://github.com/avilaops/orcamento"
Write-Host ""

# Verificar se está em um repositório Git
if (-not (Test-Path ".git")) {
    Write-Error2 "Não é um repositório Git!"
    Write-Host ""
    Write-Info "Inicializando repositório..."
    git init
    Write-Success "Repositório Git inicializado!"
}

# ============================================================
# VERIFICAR CREDENCIAIS NAS VARIÁVEIS DE AMBIENTE
# ============================================================

Write-Step "1/8" "🔐 Verificando credenciais..."

$githubToken = $env:GITHUB_TOKEN
$githubUsername = $env:GITHUB_USERNAME
$githubEmail = $env:GITHUB_EMAIL

# Verificar tokens Azure DevOps (pode ter configurado lá)
if (-not $githubToken) {
    $githubToken = $env:AZURE_DEVOPS_PAT
}

if (-not $githubToken) {
    $githubToken = $env:CARGO_REGISTRY_TOKEN
}

if ($githubToken) {
    Write-Success "Token encontrado! (primeiros 8 chars: $($githubToken.Substring(0,8))...)"
    $useToken = $true
} else {
    Write-Warning2 "Token não encontrado. Usando credenciais do sistema."
    $useToken = $false
}

Write-Host ""

# ============================================================
# CONFIGURAR GIT
# ============================================================

Write-Step "2/8" "🔧 Configurando Git..."

if ($githubUsername) {
    git config user.name "$githubUsername"
    Write-Info "User: $githubUsername"
} else {
    git config user.name "Nicolas Avila"
    Write-Info "User: Nicolas Avila (padrão)"
}

if ($githubEmail) {
    git config user.email "$githubEmail"
    Write-Info "Email: $githubEmail"
} else {
    git config user.email "contato@avila.inc"
    Write-Info "Email: contato@avila.inc (padrão)"
}

git config credential.helper wincred
Write-Info "Credential helper: wincred"

Write-Success "Git configurado!"
Write-Host ""

# ============================================================
# VERIFICAR BRANCH
# ============================================================

Write-Step "3/8" "🌿 Verificando branch..."

$currentBranch = git rev-parse --abbrev-ref HEAD 2>$null

if (-not $currentBranch) {
    Write-Info "Criando branch 'main'..."
    git checkout -b main
    $currentBranch = "main"
}

Write-Info "Branch atual: $currentBranch"
Write-Success "Branch verificada!"
Write-Host ""

# ============================================================
# VERIFICAR REMOTE
# ============================================================

Write-Step "4/8" "🔗 Configurando remote..."

$remoteUrl = git remote get-url origin 2>$null

if (-not $remoteUrl) {
    Write-Info "Remote não configurado. Adicionando..."
    
    if ($useToken) {
        $url = "https://$($githubToken)@github.com/avilaops/orcamento.git"
        git remote add origin $url
        Write-Success "Remote configurado com token!"
    } else {
        git remote add origin "https://github.com/avilaops/orcamento.git"
        Write-Success "Remote configurado!"
    }
} else {
    Write-Info "Remote já configurado: $remoteUrl"
    
    # Atualizar URL se tiver token
    if ($useToken -and $remoteUrl -notlike "*$githubToken*") {
        $url = "https://$($githubToken)@github.com/avilaops/orcamento.git"
        git remote set-url origin $url
        Write-Success "Remote atualizado com token!"
    }
}

Write-Host ""

# ============================================================
# STATUS
# ============================================================

Write-Step "5/8" "📊 Status do repositório..."
Write-Host ""

$status = git status --short
if ($status) {
    $status | ForEach-Object { Write-ColorOutput Gray "   $_" }
} else {
    Write-Info "Nenhuma alteração detectada"
}

Write-Host ""

# ============================================================
# ADD
# ============================================================

Write-Step "6/8" "➕ Adicionando arquivos..."

git add .

if ($LASTEXITCODE -ne 0) {
    Write-Error2 "Erro ao adicionar arquivos!"
    exit 1
}

Write-Success "Todos os arquivos adicionados!"
Write-Host ""

# ============================================================
# COMMIT
# ============================================================

Write-Step "7/8" "💾 Criando commit..."

# Gerar mensagem automática se não fornecida
if (-not $CommitMessage) {
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm"
    $CommitMessage = "🚀 Auto commit - $timestamp"
}

Write-Info "Mensagem: $CommitMessage"

git commit -m "$CommitMessage"

if ($LASTEXITCODE -ne 0) {
    Write-Warning2 "Nenhuma alteração para commitar"
    
    # Mostrar último commit
    $lastCommit = git log --oneline -1 2>$null
    if ($lastCommit) {
        Write-Info "Último commit: $lastCommit"
    }
} else {
    Write-Success "Commit criado!"
}

Write-Host ""

# ============================================================
# PUSH
# ============================================================

Write-Step "8/8" "📤 Enviando para GitHub..."
Write-Host ""
Write-Info "⏳ Aguarde..."
Write-Host ""

# Tentar push
git push origin $currentBranch

if ($LASTEXITCODE -ne 0) {
    Write-Warning2 "Erro no push. Tentando pull + rebase..."
    
    git pull origin $currentBranch --rebase
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Pull concluído! Tentando push novamente..."
        
        git push origin $currentBranch
        
        if ($LASTEXITCODE -ne 0) {
            Write-Error2 "Erro no push após pull!"
            
            if ($Force) {
                Write-Warning2 "Usando --force (flag -Force ativa)"
                git push origin $currentBranch --force
                
                if ($LASTEXITCODE -ne 0) {
                    Write-Error2 "Erro mesmo com --force!"
                    exit 1
                }
            } else {
                Write-Host ""
                Write-ColorOutput Red "💡 Tente executar com: -Force para forçar push"
                exit 1
            }
        }
    } else {
        Write-Error2 "Erro no pull!"
        exit 1
    }
}

# ============================================================
# SUCESSO
# ============================================================

Write-Host ""
Write-Header "✅ SUCESSO! REPOSITÓRIO ATUALIZADO!"

Write-Info "📊 Último commit:"
git log --oneline -1

Write-Host ""
Write-ColorOutput Green "🌐 Veja no GitHub:"
Write-ColorOutput Green "   https://github.com/avilaops/orcamento"

Write-Host ""
Write-Info "📁 Branch: $currentBranch"
Write-Info "📅 Data: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"

Write-Host ""
Write-Header "============================================================"

# ============================================================
# ESTATÍSTICAS
# ============================================================

if ($Verbose) {
    Write-Host ""
    Write-Step "📈" "Estatísticas do repositório"
    Write-Host ""
    
    $totalCommits = git rev-list --count HEAD 2>$null
    $authors = git shortlog -sn --all 2>$null
    $files = git ls-files | Measure-Object
    
    Write-Info "Total de commits: $totalCommits"
    Write-Info "Total de arquivos: $($files.Count)"
    
    Write-Host ""
    Write-Info "Autores:"
    $authors | ForEach-Object { Write-ColorOutput Gray "   $_" }
}

Write-Host ""
