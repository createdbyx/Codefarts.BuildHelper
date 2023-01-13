Write-Host starting
$sourcePath = New-Object -TypeName System.IO.DirectoryInfo -ArgumentList "P:\Code Projects\Codefarts.BuildHelper\Codefarts.BuildHelperConsoleApp\bin\Debug\net6.0\"
$destinationPath = New-Object -TypeName System.IO.DirectoryInfo -ArgumentList "E:\BuildHelper\"

if(!$sourcePath.Exists)
{
    Write-Host "Source Path does not exist. " + $sourcePath.FullName
    exit
}

Write-Host ("Cleaning source " + $sourcePath.FullName)
cd $sourcePath.FullName
# delete ref folders
Get-ChildItem -Include ref -Recurse -Force | Remove-Item -Force -Recurse 

if($destinationPath.Exists)
{
    Write-Host ("Cleaning destination " + $destinationPath.FullName)
    cd $destinationPath.FullName 
    Get-ChildItem -Recurse -Force | Remove-Item -Force -Recurse  
}

Write-Host "Copying"

#attrib -r ($destinationPath + "\*.*")
cd $sourcePath.FullName
Copy-Item -Path ($sourcePath.FullName + "*") -Destination $destinationPath.FullName -Recurse -Force   

Write-Host "Done!"