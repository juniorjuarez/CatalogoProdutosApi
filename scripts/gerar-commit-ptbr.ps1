# Gera sugestão de mensagem de commit em pt-BR (Conventional Commits)
# Uso: .\scripts\gerar-commit-ptbr.ps1              → mensagem curta (uma linha)
#       .\scripts\gerar-commit-ptbr.ps1 -Detalhado → mensagem detalhada (título + corpo com arquivos)

param([switch]$Detalhado)

$status = git status --short 2>$null
if (-not $status) {
    Write-Host "feat: adiciona nova funcionalidade" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "# Nenhuma alteração detectada. Use uma das mensagens acima ou edite."
    exit 0
}

$paths = ($status | ForEach-Object { ($_ -split "\s+", 2)[1] }) -join " "
$type = "chore"
$scope = ""
$desc = "atualiza projeto"
$corpoCurto = "Alteracoes diversas no projeto."

# Inferir tipo, escopo, descrição e linha de corpo
if ($paths -match "Test|\.Tests\\|Validator.*Test") { $type = "test"; $scope = "test"; $desc = "adiciona ou ajusta testes unitarios"; $corpoCurto = "Inclui cenarios de validacao, mocks ou novos casos de teste." }
elseif ($paths -match "Validator") { $type = "fix"; $scope = "validacao"; $desc = "ajusta regras de validacao de DTOs"; $corpoCurto = "Corrige ou refina mensagens e regras do FluentValidation." }
elseif ($paths -match "Controller|Service(?!s\.)") { $type = "feat"; $scope = "api"; $desc = "adiciona ou altera funcionalidade na API"; $corpoCurto = "Mudancas em controllers, services ou endpoints." }
elseif ($paths -match "Repository|Infrastructure") { $type = "refactor"; $scope = "dados"; $desc = "ajusta acesso a dados e camada de persistencia"; $corpoCurto = "Alteracoes em repositorios, DbContext ou infraestrutura." }
elseif ($paths -match "\.md$|docs?\\") { $type = "docs"; $scope = "doc"; $desc = "atualiza documentacao do projeto"; $corpoCurto = "Ajustes em README, conceitos ou guias." }
elseif ($paths -match "\.csproj|package") { $type = "chore"; $scope = "deps"; $desc = "atualiza dependencias e pacotes NuGet"; $corpoCurto = "Adicao ou atualizacao de pacotes no .csproj." }
elseif ($paths -match "Program\.cs|Middleware") { $type = "refactor"; $scope = "api"; $desc = "ajusta pipeline ou configuracao da API"; $corpoCurto = "Mudancas em Program.cs, middleware ou servicos registrados." }

$scopePart = if ($scope) { "($scope): " } else { ": " }
$titulo = "$type$scopePart$desc"

if ($Detalhado) {
    # Mensagem detalhada: título + linha em branco + corpo (lista de arquivos)
    $arquivos = $status | ForEach-Object {
        $partes = $_ -split "\s+", 2
        $estado = $partes[0]
        $arquivo = $partes[1]
        $simbolo = switch -Regex ($estado) {
            "^\s*A"  { "novo" }
            "^\s*M"  { "alterado" }
            "^\s*D"  { "removido" }
            "^\s*\?\?" { "nao rastreado" }
            default  { "alterado" }
        }
        "- $arquivo ($simbolo)"
    }
    $corpo = $arquivos -join "`n"
    $msgDetalhada = "${titulo}`n`n${corpo}"
    Write-Host ""
    Write-Host "--- Copie o bloco abaixo (título + corpo) para seu commit ---" -ForegroundColor Cyan
    Write-Host ""
    Write-Host $msgDetalhada -ForegroundColor Green
    Write-Host ""
    Write-Host "---" -ForegroundColor Cyan
    Write-Host ""
    Write-Host 'Uso: git commit (sem -m; o editor abrira - cole o bloco acima e salve)'
} else {
    # Mensagem curta (titulo + uma linha de corpo)
    Write-Host ""
    Write-Host '--- Copie as duas linhas abaixo para: git commit (ou -m "linha1" -m "linha2") ---' -ForegroundColor Cyan
    Write-Host ""
    Write-Host $titulo -ForegroundColor Green
    Write-Host $corpoCurto -ForegroundColor Green
    Write-Host ""
    Write-Host "---" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Dica: para mensagem detalhada (titulo + lista de arquivos), execute:"
    Write-Host "  .\scripts\gerar-commit-ptbr.ps1 -Detalhado" -ForegroundColor Yellow
}

Write-Host ""
Write-Host 'Tipos: feat, fix, docs, test, refactor, chore'
