$ErrorActionPreference = "Stop"

Write-Host "Building Structura (Release)..." -ForegroundColor Cyan
dotnet build -c Release

$targetDir = "$PSScriptRoot\Structura.UI\bin\Release\net48"

if (Test-Path $targetDir) {
    Write-Host "Build complete successfully." -ForegroundColor Green
    Write-Host "Opening output folder: $targetDir" -ForegroundColor Gray
    Invoke-Item $targetDir
} else {
    Write-Host "Error: Output directory not found." -ForegroundColor Red
}
