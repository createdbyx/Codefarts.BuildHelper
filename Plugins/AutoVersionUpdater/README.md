## Version Updater Plugin

Automatically updates the versions in a project file using the format YEAR.MONTH.DAY.REVISION. Updating the version multiple times on the same day updates only the revision number.

### Usage

    <updateversion ProjectFileName="$(ProjectPath)"/>

### Arguments
 
* ProjectFileName - The file path to the project file.
* file - If true will increment the file version. Default is true.
* assembly - If true will increment the assembly version. Default is true.
* package - If true will increment the package version. Default is true.
* version - If true will increment the version. Default is true.
* usecurrentdate = If true will set the year/month/day values to current date. Default is true.
* retry = If true will retry to save the project if it is locked by another process. Default is false.
* retrycount = Specifies the number of times to retry. Default is 3.
* retrydelay = Species the retry delay time in milliseconds before retrying. Default is 50ms.

### Requirements

* Project file must contain a FileVersion, AssemblyVersion or PackageVersion specified otherwise this plugin will not generate one.
* A version number must consist of 4 numbers (Example: 1.2.3.201) otherwise an error occurs.
* Version numbers are expected to be integers otherwise a parsing error will occur.

### Remarks
* Revision will be set to zero if the stored date is different from the current date.