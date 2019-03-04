#!/usr/bin/env pwsh
#requires -version 4

Param(
  [string]$Configuration="Debug"
)

$ErrorActionPreference = "Stop"
$ArtifactsDir = Join-Path $PSScriptRoot 'artifacts'
$TestResultsDir = Join-Path $ArtifactsDir 'tests'

$TargetFrameworks = @{
  "netcoreapp1.0" = ".NETCoreApp,Version=v1.0"
  "netcoreapp1.1" = ".NETCoreApp,Version=v1.1"
  "netcoreapp2.0" = ".NETCoreApp,Version=v2.0"
  "netcoreapp2.1" = ".NETCoreApp,Version=v2.1"
  "netcoreapp2.2" = ".NETCoreApp,Version=v2.2"
}

If (!$IsLinux) {
  $TargetFrameworks["net452"] = ".NETFramework,Version=v4.5.2"
}

ForEach ($TargetFramework in $TargetFrameworks.GetEnumerator()) {
  $AssemblyPath = Join-Path bin $Configuration | Join-Path -ChildPath $TargetFramework.Key
  $Assemblies = ((Get-ChildItem -Recurse *.Tests.dll | % FullName) -Match [regex]::Escape($AssemblyPath))
  If ($Assemblies.Length -gt 0) {
    dotnet vstest $Assemblies "/Framework:$($TargetFramework.Value)" "/Logger:trx;LogFileName=testresults-$($TargetFramework.Key).trx" /ResultsDirectory:$TestResultsDir
    $TestsFailed = $TestsFailed -or ($LastExitCode -ne 0)
  }
}
