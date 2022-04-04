echo starting
p:
cd "\Code Projects\Codefarts.BuildHelper\Codefarts.BuildHelperConsoleApp\bin\Debug\net5.0"
Get-ChildItem -Include ref -Recurse -Force | Remove-Item -Force -Recurse
attrib -r "E:\BuildHelper\\*.*" /s
rmdir /s /q  "E:\BuildHelper\"
xcopy "P:\Code Projects\Codefarts.BuildHelper\Codefarts.BuildHelperConsoleApp\bin\Debug\net5.0\*.*" "E:\BuildHelper\" /e /y
