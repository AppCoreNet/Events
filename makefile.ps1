Properties {
  $Configuration = ?? $Configuration 'Debug'
  $OutputDir = ?? $OutputDir (Join-ProjectPath 'artifacts')
  $PackagesDir = Join-Path $OutputDir 'packages'
  $TestResultsDir = Join-Path $OutputDir 'test-results'
}

Task Default -DependsOn Build

Task Init {
  DotNet-InstallTool gitversion.tool -version 5.0.0-beta2-32
  $GitVersion = DotNet-GitVersion
  DotNet-SetVersion $GitVersion
}

Task Restore -DependsOn Init {
  DotNet-Restore
}

Task Build -DependsOn Restore {
  DotNet-Build -Configuration $Configuration
}

Task Package -DependsOn Build {
  DotNet-Pack -Configuration $Configuration -OutputDir $PackagesDir
}

Task Publish -DependsOn Package

Task Test -DependsOn Build {
  DotNet-Test -Configuration $Configuration -OutputDir $TestResultsDir
}
