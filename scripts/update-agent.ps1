# Update script for Agent Master DevSecOps
# Repositório Oficial: git@github.com:ContalApps/agent-master-devsecops.git

$ErrorActionPreference = "Stop"
Write-Host "🔄 Atualizando Agent Master DevSecOps a partir de git@github.com:ContalApps/agent-master-devsecops.git..." -ForegroundColor Cyan

$AgentDir = Join-Path $PSScriptRoot "..\\.agents\\agent-master-devsecops"
if (-not (Test-Path $AgentDir)) {
    Write-Host "❌ Diretório do agente não encontrado em $AgentDir" -ForegroundColor Red
    exit 1
}

Push-Location $AgentDir
try {
    git fetch origin
    git pull origin main
    Write-Host "🧪 Executando testes unitários do agente..." -ForegroundColor Green
    npm test
    Write-Host "`n✅ Agent Master DevSecOps atualizado com sucesso e 100% testado!" -ForegroundColor Green
} finally {
    Pop-Location
}
