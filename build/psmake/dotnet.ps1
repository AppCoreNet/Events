function DotNet-InstallTool {
  [CmdletBinding()]
  param(
    [Parameter(Position=0, Mandatory=$true)]
    [string]$Name,
    [string]$Version
  )

  $ToolsDir = Join-ProjectPath "build/dotnet-tools"
  $ToolsStore = Join-Path $ToolsDir ".store"
 
  $ToolDir = Join-Path $ToolsStore $Name
  $ToolExists = Test-Path $ToolDir
  
  If (!$ToolExists) {
    Write-Information "-- Installing tool $($Name) $($Version) ---"
    $Output = Exec dotnet tool install --tool-path $ToolsDir --version $Version $Name | Out-String
    Write-Verbose $Output
  }
}

function DotNet-SetVersion {
  [CmdletBinding()]
  param(
    [Parameter(Position=0, Mandatory=$true)]
    $Version
  )
  
  $VersionProps = @"
  <Project>
    <PropertyGroup>
      <Version>$($Version.SemVer)</Version>
      <VersionPrefix>$($Version.MajorMinorPatch)</VersionPrefix>
      <VersionSuffix>$($Version.PreReleaseTag)</VersionSuffix>
      <AssemblyVersion>$($Version.AssemblySemVer)</AssemblyVersion>
      <InformationalVersion>$($Version.FullSemVer)+$($Version.Sha)</InformationalVersion>
      <FileVersion>$($Version.AssemblySemFileVer)</FileVersion>
      <PackageVersion>$($Version.FullSemVer)</PackageVersion>
    </PropertyGroup>
  </Project>
"@

  $VersionPropsPath = Join-ProjectPath "build/version.props"

  If (Test-Path $VersionPropsPath) {
    $CurrentVersionProps = Get-Content $VersionPropsPath -Raw
  }

  If ($CurrentVersionProps -ne $VersionProps) {
    Write-Verbose "Updated version.props: $VersionProps"
    Out-File $VersionPropsPath -InputObject $VersionProps -NoNewline -Encoding UTF8
  }
  
  # Azure DevOps build
  If ($env:TF_BUILD) {
    Write-Host "##vso[build.updatebuildnumber]$($Version.FullSemVer)"
  }
}

function DotNet-Restore {
  [CmdletBinding()]
  param()

  Write-Information "--- Restoring NuGet packages ---"
  Exec dotnet restore
}

function DotNet-Build {
  [CmdletBinding(PositionalBinding=$false)]
  param(
    $Configuration = "Debug"
  )

  Write-Information "--- Building .NET projects ---"
  Exec dotnet build -c $Configuration --no-restore
}

function DotNet-Pack {
  [CmdletBinding()]
  param(
    [Parameter(Position=0)]
    $OutputDir = "artifacts",
    $Configuration = "Debug"
  )

  If (![System.IO.Path]::IsPathRooted($OutputDir)) {
    $OutputDir = Join-Path $BaseDir $OutputDir
  }
  
  Write-Information "--- Packing NuGet packages ---"
  Exec dotnet pack -c $Configuration --no-build -o $OutputDir
}

function DotNet-Test {
  [CmdletBinding()]
  param(
    [Parameter(Position=0)]
    $OutputDir = "test-results",
    $Configuration = "Debug"
  )

  If (![System.IO.Path]::IsPathRooted($OutputDir)) {
    $OutputDir = Join-Path $BaseDir $OutputDir
  }

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

  Write-Information "--- Running .NET unit tests ---"
  ForEach ($TargetFramework in $TargetFrameworks.GetEnumerator()) {
    $AssemblyPath = Join-Path bin $Configuration | Join-Path -ChildPath $TargetFramework.Key
    $Assemblies = ((Get-ChildItem -Recurse *.Tests.dll | % FullName) -Match [regex]::Escape($AssemblyPath))
    If ($Assemblies.Length -gt 0) {
      Exec dotnet vstest @Assemblies "/Framework:$($TargetFramework.Value)" "/Logger:trx;LogFileName=$($TargetFramework.Key).trx" /ResultsDirectory:$OutputDir
      $TestsFailed = $TestsFailed -or ($LastExitCode -ne 0)
    }
  }
}
