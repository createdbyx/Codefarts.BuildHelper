## Version Updater Plugin

### Usage

    <updateversion ProjectFileName="$(ProjectPath)"/>

### Arguments
 
* ProjectFileName - The file path to the project file.
* file - If true will increment the file version. Default is true.
* assembly - If true will increment the assembly version. Default is true.
* package - If true will increment the package version. Default is true.

### Requirements

* Project file must contain a FileVersion, AssemblyVersion or PackageVersion specified otherwise this plugin will not generate one.
* A version number must consist of 4 numbers (Example: 1.2.3.201) otherwise an error occurs.