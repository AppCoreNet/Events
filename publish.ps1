#!/usr/bin/env pwsh
#requires -version 5
Import-Module (Join-Path $PSScriptRoot "build/psmake")
Invoke-PsMake -Targets Publish @args
