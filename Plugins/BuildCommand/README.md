## Build Plugin

Sets up build varibles then runs any child commands. This command is intended to
be run as part of a pre or post build process.

### Example project data

    <Target Name="PostBuild" AfterTargets="PostBuildEvent">
        <Exec Command="buildhelper -b:'$(ProjectDir)$(ConfigurationName)-PostBuild.xml' -p:'$(ProjectPath)' -tf:'$(TargetFramework)'" />
    </Target>
    <Target Name="PostBuild" AfterTargets="PreBuildEvent">
        <Exec Command="buildhelper -b:'$(ProjectDir)$(ConfigurationName)-PreBuild.xml' -p:'$(ProjectPath)' -tf:'$(TargetFramework)'" />
    </Target>

### Usage Debug-PostBuild.xml

       <build>
           ... child commands go here ... 
       </build> 

            
### Requirements

* Needs a registered IConfigurationProvider implementation.
* The IConfigurationProvider implementation needs to contain 
"filename" as a full path to the build file being run, "projectfile"
as a full path to the project file being built, and "targetframework"
set to the name of the target framework that the project is being build for.
Given these three settings most build variables can be generated.


