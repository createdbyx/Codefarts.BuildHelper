## Excel Model Builder Plugin

Extracts data from Excel

### Usage

       <excellmodelexport source="$(ProjectDir)$(OutDir)"
             destination="$(ProjectDir)ExcellModels\" 
             singlefile="true" namespace="Codefarts" />


### Arguments
 
* source - The source excel file that will be processed.
* destination - The destination folder where model files will be generated in.
* singlefile - If true will generate individual code files whose filename and class name match the table names. Default is false.
* namespace - Specifies the namespace where the generated classes will be located.

### Requirements

* Source and destination paths cannot contain invalid path characters or an error will occur.
* If singlefile argument is true then the destination argument must point to a filename that ends with "~~~~.cs".
~~~~