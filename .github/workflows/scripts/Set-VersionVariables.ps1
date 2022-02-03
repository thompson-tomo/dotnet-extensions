# Requires -Version 7
# Requires -Moduels powershell-devops

[CmdletBinding()]
param
(
    [ValidateNotNullOrEmpty()]
    [string] $GITHUB_REF = $(Get-EnvironmentVariable GITHUB_REF -Required)
)

$Version = $($GITHUB_REF -replace '^refs/tags/v','')
$IsPrerelease = $Version -like '*-*' ? 'true' : 'false'

Write-Host "Version: '$Version' | IsPrerelease: $IsPrerelease"

Set-EnvironmentVariable PACKAGE_VERSION $Version
Set-EnvironmentVariable IS_PRERELEASE $IsPrerelease
