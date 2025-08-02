param ([string]$vs_ProjectDir,[string]$vs_TargetFramework, [string]$vs_ConfigurationName,[string]$vs_OutDir,[string]$vs_ProjectName,[string]$vs_SolutionDir)

$vs_TargetFramework   = $vs_TargetFramework.Trim("'") 
$vs_ConfigurationName = $vs_ConfigurationName.Trim("'")
$vs_SolutionDir       = $vs_SolutionDir.Trim("'")
$vs_ProjectDir        = $vs_ProjectDir.Trim("'")
$vs_OutDir            = $vs_OutDir.Trim("'")
$vs_ProjectName       = $vs_ProjectName.Trim("'")

# check if target framework specified
if ( [string]::IsNullOrWhiteSpace($vs_TargetFramework))
{
    Write-Error "Target framework is not specified"
    exit 1
}

# check if  solution directory specified
if ( [string]::IsNullOrWhiteSpace($vs_SolutionDir))
{
    Write-Error "Solution directory is not specified"
    exit 1
}

# check if project directory specified
if ( [string]::IsNullOrWhiteSpace($vs_ProjectDir))
{
    Write-Error "Project directory is not specified"
    exit 1
}

$source = ($vs_ProjectDir + $vs_OutDir + $vs_ProjectName + ".*").Trim("'")
$destination = ($vs_SolutionDir + "Codefarts.BuildHelperConsoleApp/bin/" + $vs_ConfigurationName + "/" + $vs_TargetFramework + "/Plugins/" + $vs_ProjectName + "/").Trim("'")


#Write-Output ( "vs_TargetFramework -- "        + $vs_TargetFramework     )
#Write-Output ( "vs_ConfigurationName -- "        + $vs_ConfigurationName  )
#Write-Output ( "vs_SolutionDir -- "        + $vs_SolutionDir              )
#Write-Output ( "vs_ProjectDir  -- "        + $vs_ProjectDir               )
#Write-Output ( "vs_OutDir -- "        + $vs_OutDir                        )
#Write-Output ( "vs_ProjectName -- "        + $vs_ProjectName              )
#Write-Output ( "destination -- "        + $destination                  )
#Write-Output ( "source -- "        + $source                            )
#exit;
# $destination = '$(SolutionDir)Codefarts.BuildHelperConsoleApp/bin/$(ConfigurationName)/$(TargetFramework)/Plugins/$(ProjectName)/'; 
if (-Not (Test-Path $destination)) { 
    New-Item -ItemType Directory -Path $destination -Force 
}; 
Copy-Item $source $destination -Force -Recurse
#Copy-Item '$(ProjectDir)$(OutDir)$(ProjectName).*' $destination -Force -Recurse
