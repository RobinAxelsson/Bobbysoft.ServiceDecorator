# Taken from automapper https://github.com/AutoMapper/AutoMapper.git

<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>

param(
    [string]$Framework
)

function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

$artifacts = ".\artifacts"
if(Test-Path $artifacts) { Remove-Item $artifacts -Force -Recurse }

$slnpath = ".\src\libraries\Bobbysoft.Extensions.DependencyInjection.Decoration\Bobbysoft.Extensions.DependencyInjection.Decoration.sln"
exec { & dotnet test $slnpath -c Release --results-directory $artifacts -l trx --verbosity=normal -f $Framework }

$projectpath = ".\src\libraries\Bobbysoft.Extensions.DependencyInjection.Decoration\src\Bobbysoft.Extensions.DependencyInjection.Decoration.csproj"
exec { & dotnet pack $projectpath -c Release -o $artifacts --no-build }
