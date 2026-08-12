[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidateSet('MSTestX.UnitTestRunner', 'MSTestX.TestAdapter')]
    [string] $PackageId,
    [Parameter(Mandatory)]
    [string] $Version,
    [Parameter(Mandatory)]
    [string] $OutputDirectory,
    [string] $CertificatePath = $env:MSTESTX_CERTIFICATE_PATH,
    [string] $CertificatePassword = $env:MSTESTX_CERTIFICATE_PASSWORD,
    [string] $SignToolPath = $env:SignToolPath
)
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$semVerPattern = '^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[A-Za-z-][0-9A-Za-z-]*)(?:\.(?:0|[1-9]\d*|\d*[A-Za-z-][0-9A-Za-z-]*))*))?$'
$versionMatch = [regex]::Match($Version, $semVerPattern)
if (-not $versionMatch.Success) {
    throw "Version '$Version' must be strict SemVer without build metadata."
}

$numericParts = @($versionMatch.Groups[1..3] | ForEach-Object {
    if ($_.Value.Length -gt 5 -or [int]$_.Value -gt 65534) {
        throw "Version '$Version' cannot produce a valid numeric assembly version."
    }
    [int]$_.Value
})
$numericVersion = '{0}.{1}.{2}.0' -f $numericParts
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectRelativePath = switch ($PackageId) {
    'MSTestX.UnitTestRunner' { 'src/TestAppRunner/TestAppRunner/TestAppRunner.csproj' }
    'MSTestX.TestAdapter' { 'src/MSTestX.Adapter/MSTestX.Adapter.csproj' }
}
$projectPath = Join-Path $repositoryRoot $projectRelativePath
$fullOutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $fullOutputDirectory) {
    if (-not (Test-Path -LiteralPath $fullOutputDirectory -PathType Container)) {
        throw "OutputDirectory '$fullOutputDirectory' is not a directory."
    }
    if (@(Get-ChildItem -LiteralPath $fullOutputDirectory -Force).Count -ne 0) {
        throw "OutputDirectory '$fullOutputDirectory' must be empty."
    }
}
else {
    $null = New-Item -ItemType Directory -Path $fullOutputDirectory
}
$certificateSupplied = -not [string]::IsNullOrWhiteSpace($CertificatePath)
$passwordSupplied = -not [string]::IsNullOrWhiteSpace($CertificatePassword)
if ($certificateSupplied -ne $passwordSupplied) {
    throw 'CertificatePath and CertificatePassword must both be supplied for signing.'
}
if ($certificateSupplied) {
    $CertificatePath = [IO.Path]::GetFullPath($CertificatePath)
    if (-not (Test-Path -LiteralPath $CertificatePath -PathType Leaf)) {
        throw "Certificate file '$CertificatePath' does not exist."
    }

    if ([string]::IsNullOrWhiteSpace($SignToolPath)) {
        if (-not $IsWindows) {
            throw 'Automatic SignTool discovery is available only on Windows.'
        }
        $kitsBin = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\bin'
        $SignToolPath = Get-ChildItem -LiteralPath $kitsBin -Directory -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -as [version] } |
            Sort-Object { [version]$_.Name } -Descending |
            ForEach-Object { Join-Path $_.FullName 'x64\signtool.exe' } |
            Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } |
            Select-Object -First 1
    }
    if ([string]::IsNullOrWhiteSpace($SignToolPath) -or
        -not (Test-Path -LiteralPath $SignToolPath -PathType Leaf)) {
        throw 'signtool.exe was not found. Install a Windows SDK or supply SignToolPath.'
    }
    $SignToolPath = [IO.Path]::GetFullPath($SignToolPath)
}
function Invoke-DotNet([string[]] $Arguments) {
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}
$properties = @(
    "-p:Version=$Version"
    "-p:PackageVersion=$Version"
    "-p:AssemblyVersion=$numericVersion"
    "-p:FileVersion=$numericVersion"
    '-p:ContinuousIntegrationBuild=true'
    '-p:GeneratePackageOnBuild=false'
)
if ($PackageId -eq 'MSTestX.UnitTestRunner') {
    $properties += '-p:NuGetAdapter=true'
}
if ($certificateSupplied) {
    $properties += "-p:SignToolPath=$SignToolPath"
}
$oldCertificatePath = $env:CertificatePath
$oldPassword = $env:MSTESTX_PFX_PASSWORD
try {
    if ($certificateSupplied) {
        $env:CertificatePath = $CertificatePath
        $env:MSTESTX_PFX_PASSWORD = $CertificatePassword
    }
    if ($PackageId -eq 'MSTestX.UnitTestRunner') {
        Invoke-DotNet (@('workload', 'restore', $projectPath) + $properties)
    }
    Invoke-DotNet (@('restore', $ProjectPath, '-p:Configuration=Release') + $properties)
    Invoke-DotNet (@('pack', $projectPath, '--configuration', 'Release', '--no-restore',
        '--output', $fullOutputDirectory) + $properties)
}
finally {
    $env:CertificatePath = $oldCertificatePath
    $env:MSTESTX_PFX_PASSWORD = $oldPassword
}
$packages = @(Get-ChildItem -LiteralPath $fullOutputDirectory -Filter '*.nupkg' -File)
$symbolPackages = @(Get-ChildItem -LiteralPath $fullOutputDirectory -Filter '*.snupkg' -File)
if ($packages.Count -ne 1 -or $symbolPackages.Count -ne 1) {
    throw "Expected one nupkg and one snupkg; found $($packages.Count) and $($symbolPackages.Count)."
}
if (-not [string]::IsNullOrWhiteSpace($env:GITHUB_OUTPUT)) {
    Add-Content -LiteralPath $env:GITHUB_OUTPUT -Value "nupkg_path=$($packages[0].FullName)" -Encoding utf8
    Add-Content -LiteralPath $env:GITHUB_OUTPUT -Value "snupkg_path=$($symbolPackages[0].FullName)" -Encoding utf8
}
