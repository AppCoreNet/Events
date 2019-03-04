#!/usr/bin/env pwsh
#requires -version 4

Param(
  [string]$Configuration="Debug",
  [string]$VersionSuffix="alpha",
  [string]$BuildNumber="",
  [switch]$CI=$false
)

$ErrorActionPreference = "Stop"

$ArtifactsDir = Join-Path $PSScriptRoot 'artifacts'
$ExtraArgs = @()
$ExtraBuildArgs = @()

If ($BuildNumber) {
  $VersionSuffix += ".$($BuildNumber)"
}

If ($VersionSuffix) {
	$ExtraBuildArgs += "--version-suffix", $VersionSuffix
}

If ($CI) {
	$ExtraArgs += "/p:CI=true"
}

If ($Configuration) {
	$ExtraBuildArgs += "--configuration", "$Configuration"
}

dotnet restore $ExtraArgs
If ($LastExitCode -ne 0) { throw "Package restore failed." }

dotnet build --no-restore @ExtraBuildArgs $ExtraArgs
If ($LastExitCode -ne 0) { throw "Build failed." }

dotnet pack --no-build --no-restore @ExtraBuildArgs -o $ArtifactsDir $ExtraArgs
If ($LastExitCode -ne 0) { throw "Packaging failed." }
