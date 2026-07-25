<#
.SYNOPSIS
    Publishes Bloom as a self-contained, single-file executable.

.DESCRIPTION
    Wraps `dotnet publish` with the Portable publish profiles. The output requires no
    .NET runtime on the target machine; the only files beside Bloom.exe are the loose
    art assets under Assets\Art, which are read from disk at runtime.

.PARAMETER Runtime
    Target runtime identifier. One of win-x64, win-x86, win-arm64, or all.

.PARAMETER Clean
    Delete the publish directory before building.

.EXAMPLE
    .\publish.ps1
    .\publish.ps1 -Runtime all -Clean
#>
[CmdletBinding()]
param(
    [ValidateSet('win-x64', 'win-x86', 'win-arm64', 'all')]
    [string] $Runtime = 'win-x64',

    [switch] $Clean
)

$ErrorActionPreference = 'Stop'

[string] $root = $PSScriptRoot
[string] $project = Join-Path $root 'Bloom.csproj'

[string[]] $targets = if ($Runtime -eq 'all') { @('win-x64', 'win-x86', 'win-arm64') } else { @($Runtime) }

foreach ($target in $targets) {
    [string] $profileName = "Portable-$target"
    [string] $outputDir = Join-Path $root "bin\Publish\$target"

    if ($Clean -and (Test-Path $outputDir)) {
        Write-Host "Cleaning $outputDir" -ForegroundColor DarkGray
        Remove-Item -Recurse -Force $outputDir
    }

    Write-Host "Publishing $target ($profileName)..." -ForegroundColor Cyan
    dotnet publish $project -c Portable -p:PublishProfile=$profileName --nologo
    if ($LASTEXITCODE -ne 0) {
        throw "Publish failed for $target (exit code $LASTEXITCODE)."
    }

    [string] $exe = Join-Path $outputDir 'Bloom.exe'
    if (Test-Path $exe) {
        [double] $megabytes = (Get-Item $exe).Length / 1MB
        Write-Host ("  {0}  ({1:N1} MB)" -f $exe, $megabytes) -ForegroundColor Green
    }
}
