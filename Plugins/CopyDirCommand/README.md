## CopyDir Plugin

Copies files from one directory to another.

### Usage

       <copydir source="$(ProjectDir)$(OutDir)"
             destination="P:\Codefarts Assemblies\$(ProjectName)\Latest\$(ConfigurationName)\" clean="true"/>


### Arguments
 
* source - The source folder that will be copied.
* destination - The destination folder where files and folder will be copied to.
* clean - If true will delete contents from the destination before copying. Default is false.
* subfolders - If true will copy subfolders as well. Default is true.
* allconditions - Specifies weather or not all conditions must be satisfied. Default is false.
* ignoreconditions - Specifies weather to ignore conditions. Default is false.
* test - Specifies weather to run in test mode. Default is false.

### Requirements

* Source and destination paths cannot contain invalid path characters or an error will occur.
~~~~