# run-tests.ps1

# Chemins
$coverageOutput = "coverage"
$coverageReport = "$coverageOutput\report"

# Créer le dossier s'il n'existe pas
if (-Not (Test-Path $coverageOutput)) {
    New-Item -ItemType Directory -Path $coverageOutput | Out-Null
}

# Exécuter les tests avec collecte de couverture
dotnet test .\StacktimApi.Tests\StacktimApi.Tests.csproj --collect:"XPlat Code Coverage" --results-directory $coverageOutput

# Trouver le fichier de couverture généré
$coverageFile = Get-ChildItem -Path $coverageOutput -Recurse -Filter "coverage.cobertura.xml" | Select-Object -First 1

# Générer un rapport HTML avec ReportGenerator
ReportGenerator -reports:$coverageFile.FullName -targetdir:$coverageReport -reporttypes:Html

# Ouvrir le rapport dans le navigateur
Start-Process "$coverageReport\index.html"

Write-Host "Rapport de couverture généré et ouvert dans le navigateur."
