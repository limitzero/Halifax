#-------------------------------------------------------------------------------------------------
# Function to invoked to clean of a solution via MSBuild
#-------------------------------------------------------------------------------------------------
function Invoke-Clean
{  [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)] [string]$BuildName = $null,
        [Parameter(Position=1,Mandatory=1)] [string]$SolutionFileWithPath = $null
        )
    
    Write-Host "Running Clean for" $BuildName "on Solution @" $SolutionFileWithPath
    
    C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /verbosity:m /t:Clean $SolutionFileWithPath
}
#-------------------------------------------------------------------------------------------------
# Function to invoked to build of a solution via MSBuild
#-------------------------------------------------------------------------------------------------
function Invoke-Build
{  
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)] [string]$BuildName = $null,
        [Parameter(Position=1,Mandatory=1)] [string]$SolutionFileWithPath = $null
        )
    
    Write-Host "Running Build for" $BuildName "on Solution @" $SolutionFileWithPath
    
    C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /verbosity:m $SolutionFileWithPath
}
#-------------------------------------------------------------------------------------------------
# Function to call MSTest for running the unit tests against an assembly
#-------------------------------------------------------------------------------------------------
function Invoke-MSTest
{
  [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)] [string]$TestName = $null,
        [Parameter(Position=1,Mandatory=1)] [string]$TestDll = $null,
        [Parameter(Position=2,Mandatory=1)] [string]$ResultTrx = $null
        )
        
    Write-Host "Running Tests for" $TestName "Located @" $TestDll " Output to:" $ResultTrx
    
    if ( Test-Path $ResultTrx )       
    {
        Write-Host "Found" $ResultTrx "it will be deleted prior to running the test"
        Remove-Item $ResultTrx
    }       
    
    $result = mstest /testcontainer:$TestDll /resultsfile:$ResultTrx
       
    foreach( $line in $result )
    {
        if ( $line -ne $null )
        {
            if ( $line.StartsWith("Passed") -ne $true )
            {
                $line
            }
        }
    }
}