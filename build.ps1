[CmdletBinding()]
Param([Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)][string[]]$arguments)
# Version of the Builder package that is bootstrapped by this bootstrapper script
$majorVersion = "3"
$package ="Kongsberg.KSim.Builder"
# Find latest version of the Builder package on the NuGet server
$nugetSource = "http://nugetsim.partner.master.int/NuGetSim/nuget"
$nugetSourceOrg ="https://api.nuget.org/v3/index.json"
 $version = ((& "NuGet.exe" list $package -AllVersions -source $nugetSource) | Out-String) -split "`n" `
     | ? { $_ -match "$package ($majorVersion\.[0-9\.]+)" } `
     | % { $matches[1] } `
     | Sort-Object { [Version]$_ } `
     | Select -Last 1
 if (-not $version) {
     Throw "Cannot find $package version $majorVersion on $nugetSource"
 }
 Write-Host "$package.$version found on $nugetSource"
# Try to find locally installed Builder package
$toolsRoot = "$PSScriptRoot/.tools-packages"
New-Item -ItemType Directory -Force -Path $toolsRoot | Out-Null
$localVersion = Get-Item "$toolsRoot/$package.*" `
    | % { $_.Name } `
    | ? { $_ -match "$package\.$majorVersion\.(.+)$" } `
    | % { "$majorVersion.$($matches[1])" } `
    | Sort-Object { [Version]$_ } `
    | Select -Last 1
if ($localVersion) {
    Write-Host "$package.$localVersion found in $toolsRoot"
} else {
    Write-Host "$package.$majorVersion.* not found in $toolsRoot"
}
# If needed, install the latest version of the package
if ((-not $localVersion) -or ($version -ne $localVersion)) {
    $result = & "NuGet.exe" install $package -source $nugetSource -fallbacksource $nugetSourceOrg -OutputDirectory "$toolsRoot/" -Version $version
    Write-Host "$package.$version installed to $toolsRoot"
}
# Check that the Builder executable is present where it should be
$buildExePath = "$toolsRoot/$package.$version/tools/Builder.exe"
if (!(Test-Path $buildExePath)) {
    Throw "Could not find $buildExePath"
}
# Invoke Builder.exe from the bootstrapped NuGet package
if ($DebugPreference -eq "Inquire") { $arguments += "-debug" }
& $buildExePath (@($PSScriptRoot) + $arguments)
