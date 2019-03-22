function DotNet-GitVersion {
  [CmdletBinding()]
  param()
  
  $GitVersionPath = $ToolsDir = Join-ProjectPath "build/dotnet-tools/dotnet-gitversion"
  Exec $GitVersionPath | ConvertFrom-Json
}
