<#
.SYNOPSIS
    Orquestrador de Migrations - Executa novos scripts e ignora os já aplicados.
#>

param(
    [string]$Servidor = ".\SQLEXPRESS",
    [string]$Banco = "Pomodoro"
)

$ErrorActionPreference = "Stop"

function Executar-Pasta([string]$caminhoPasta, [bool]$especificarBanco = $true) {
    if (-not (Test-Path $caminhoPasta)) { return }

    Write-Host "`n==========================================" -ForegroundColor Yellow
    Write-Host " Processando: $caminhoPasta" -ForegroundColor Yellow
    Write-Host "==========================================" -ForegroundColor Yellow

    Get-ChildItem -Path "$caminhoPasta\*.sql" | Sort-Object Name | ForEach-Object {
        $arquivo = $_.Name
        $caminhoArquivo = $_.FullName
        Write-Host " -> Verificando: $arquivo..." -NoNewline

        try {
            if ($especificarBanco) {
                $resultado = sqlcmd -S $Servidor -d $Banco -E -i "$caminhoArquivo" -b 2>&1
            } else {
                $resultado = sqlcmd -S $Servidor -E -i "$caminhoArquivo" -b 2>&1
            }

            if ($resultado -match "já aplicada") {
                Write-Host " [JÁ APLICADA / IGNORADA]" -ForegroundColor DarkGray
            } else {
                Write-Host " [NOVA MIGRATION APLICADA!]" -ForegroundColor Green
            }
        }
        catch {
            Write-Host " [ERRO]" -ForegroundColor Red
            Write-Error "Falha ao executar $arquivo.`nDetalhes: $_"
            exit 1
        }
    }
}

# --- FLUXO DE EXECUÇÃO ---
Write-Host "Iniciando verificação do banco '$Banco'..." -ForegroundColor Cyan

Executar-Pasta ".\Setup" $false
Executar-Pasta ".\Migrations" $true

Write-Host "`nBanco de dados atualizado com sucesso!" -ForegroundColor Green

# -------------------------------------------------------------
# REVERSE ENGINEERING AUTOMÁTICO (EF CORE SCAFFOLD)
# -------------------------------------------------------------
Write-Host "`n==========================================" -ForegroundColor Yellow
Write-Host " Executando Reverse Engineer (EF Core)..." -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow

# Caminho para o projeto C# PomodoroDev
$projetoCsharp = "..\PomodoroDev"

if (Test-Path $projetoCsharp) {
    try {
        $connectionString = "Server=$Servidor;Database=$Banco;Trusted_Connection=True;TrustServerCertificate=True;"
        
        # Executa o scaffold apontando para a pasta do PomodoroDev
        dotnet ef dbcontext scaffold $connectionString Microsoft.EntityFrameworkCore.SqlServer `
            --project $projetoCsharp `
            --startup-project $projetoCsharp `
            --output-dir Entities `
            --context-dir Context `
            --context PomodoroDbContext `
            --no-onconfiguring `
            --force

        Write-Host " [OK] Entidades C# atualizadas em $projetoCsharp\Entities!" -ForegroundColor Green
    }
    catch {
        Write-Host " [ERRO] Falha ao rodar o scaffold do EF Core." -ForegroundColor Red
        Write-Error $_
    }
} else {
    Write-Host " [AVISO] Pasta do projeto C# não encontrada em $projetoCsharp. Scaffold pulado." -ForegroundColor Yellow
}