## Version Updater Plugin

Automatically updates the versions in a project file using the format YEAR.MONTH.DAY.REVISION. Updating the version multiple times on the same day updates only the revision number.

### Usage

    <powershell file="c:\script.ps"/>
    <powershell>
        #some script code
    </powershell>

### Arguments
 
* file - Specifies the location of a powershell script.

### Requirements


### Remarks
* If both a script file is specified as well as a embeded scipt the script file will be executed first.