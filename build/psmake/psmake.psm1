$ErrorActionPreference = "Stop"
$InformationPreference = "Continue"

$BaseDir = Join-Path $PSScriptRoot .. | Join-Path -ChildPath .. -Resolve
$BuildDir = Join-Path $BaseDir build

Get-ChildItem -Path $PSScriptRoot -Filter *.ps1 | % {
  $ThisScriptPath = Join-Path $PSScriptRoot "init.ps1"

  If ($_.FullName -ne $ThisScriptPath) {
    . $_.FullName
  }
}

function Exec() {
  $Command, $Arguments = $args
  & $Command $Arguments
  
  If ($LastExitCode -ne 0) {
    Write-Error "Error executing command: '$($Command) $($Arguments)'"
  }
}

$script:Context = New-Object System.Collections.Queue

function Task {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory = $true)]
    [string]$Name,
    [scriptblock]$Action = $null,
    [scriptblock]$PreAction = $null,
    [scriptblock]$PostAction = $null,
    [scriptblock]$PreCondition = {$true},
    [scriptblock]$PostCondition = {$true},
    [switch]$ContinueOnError,
    [string[]]$DependsOn = @()
  )
  
  $NewTask = @{
    Name = $Name
    DependsOn = $DependsOn
    PreAction = $PreAction
    Action = $Action
    PostAction = $PostAction
    PreCondition = $PreCondition
    PostCondition = $PostCondition
    ContinueOnError = $ContinueOnError
    ElapsedTime = New-Object System.TimeSpan
  }
  
  $TaskKey = $Name.ToLower()  
  $script:Context.Peek().Tasks[$TaskKey] = $NewTask
}

function Properties {
  [CmdletBinding()]
  param(
    [scriptblock]$Values = $null
  )
  $script:Context.Peek().Properties = $Values
}

function Invoke-Task {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory = $true)]
    [string]$Name
  )
  
  $Context = $script:Context.Peek()
  $TaskKey = $Name.ToLower()
  $Task = $Context.Tasks[$TaskKey]
  
  If (!$Task) {
    Write-Error "No task with name '$($Name)' is defined."
    return
  }
  
  If ($Context.InvokedTasks.Contains($TaskKey)) {
    return
  }
  
  If ($Context.TaskStack.Contains($TaskKey)) {
    Write-Error "Cirular reference for task '$($Task.Name)'."
    return
  }

  $Stopwatch = New-Object System.Diagnostics.Stopwatch
  $Context.TaskStack.Enqueue($TaskKey)
  Try
  {
    ForEach($ChildTask in $Task.DependsOn) {
      Invoke-Task $ChildTask
    }
    
    Write-Verbose "Invoking task '$($Task.Name)'"
    $Stopwatch.Start()
    Try {
      If ($Task.Action) {
        & $Task.Action
      }
    }
    Catch {
      If ($Task.ContinueOnError) {
        Write-Warning $_
      }
      Else {
        Write-Error "Task '$($Task.Name)' failed: $_" -ErrorAction 'Continue'
        Write-Error $_
      }
    }
    Finally {
      $Stopwatch.Stop()
      $Task.ElapsedTime = $Stopwatch.Elapsed
      Write-Verbose "Task '$($Task.Name)' finished in $($Task.ElapsedTime.TotalSeconds) secs."
    }
  }
  Finally {
    $Context.TaskStack.Dequeue() | Out-Null
    $Context.InvokedTasks += $TaskKey
  }
}

function Load-Makefile {
  Param(
    [Parameter(Mandatory = $true)]
    [string]$Path
  )
  
  $Path = Resolve-Path $Path
  . $Path
  $Context = $script:Context.Peek().Makefile = $Path
 
  Write-Verbose "Loaded $(Resolve-Path $Path)"
}

function Get-Makefile {
  $Context = $script:Context.Peek()
  return $Context.Makefile 
}

function Join-ProjectPath {
  Param(
    [Parameter(Mandatory = $true, ValueFromRemainingArguments = $true)]
    [string[]]$ChildPaths
  )

  $Path = Split-Path (Get-Makefile) -Parent
  $ChildPaths | %{ $Path = Join-Path $Path $_ }
  return $Path
}

function Invoke-PsMake {
  [CmdletBinding()]
  Param(
    [string]$Path = "makefile.ps1",
    [string[]]$Targets = @("default"),
    [hashtable]$Properties = @{}
  )

  $Context = @{
    Makefile = ''
    Properties = {}
    Tasks = @{}
    TaskStack = New-Object System.Collections.Queue
    InvokedTasks = @()
  }
  
  $script:Context.Enqueue($Context)
  Try
  {
    Load-Makefile $Path
    
    $vars = & {
    
      function Coalesce($a, $b) { if ($a -ne $null) { $a } else { $b } }
      New-Alias "??" Coalesce

      function IfTrue($a, $b, $c) { if ($a) { $b } else { $c } }
      New-Alias "?:" IfTrue

      $current = Get-Variable -Scope Local | % { $_.Name }
      $current += "current", "foreach", "property"

      ForEach($property in $Properties.GetEnumerator()) {
        Set-Variable -Name $property.Key -Value $property.Value
      }
      
      . $Context.Properties | Out-Null
      
      Get-Variable -Scope Local | Where { $current -notcontains $_.Name }
    }
    
    $vars | % {
      Set-Variable -Name $_.Name -Value $_.Value
      Write-Verbose "$($_.Name) = $($_.Value)"
    }
    
    ForEach ($Target in $Targets) {
      Invoke-Task $Target
    }
  }
  Catch {
    $Error = $_
  }

  Write-Information ""
  Write-Information "Task execution summary:"
  Write-Information "======================="
  
  $Context.InvokedTasks | % {
    $Task = $Context.Tasks[$_]
    Write-Information "$($Task.Name.PadRight(16)): $("{0:0.00}" -f $Task.ElapsedTime.TotalSeconds)s"
  }

  $script:Context.Dequeue() | Out-Null

  Write-Information ""
  If ($Error) {
    Write-Information "BUILD FAILED!"
    Write-Error "Make target '$($Target)' failed: $Error"
  }
  Else {
    Write-Information "BUILD SUCCEEDED!"
  }
}

Export-ModuleMember -Function Invoke-PsMake
