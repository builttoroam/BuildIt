@echo off
rem copy swagger json to generated folder
rem pass namespace as an argument e.g. Swagger.bat {YOUR_NAMESPACE}
rem eg Swagger.bat BtrTemplate.Client
if exist "%userprofile%\.nuget\packages\AutoRest\0.17.3\tools\AutoRest.exe" (
	echo "%userprofile%\.nuget\packages\AutoRest\0.17.3\tools\AutoRest.exe" -namespace %1 -Input http://localhost:10739/swagger/v1/swagger.json -o "%CD%\Generated"
	call "%userprofile%\.nuget\packages\AutoRest\0.17.3\tools\AutoRest.exe" -namespace %1 -Input http://localhost:10739/swagger/v1/swagger.json -o "%CD%\Generated"
	rem call "%userprofile%\.nuget\packages\AutoRest\0.17.3\tools\AutoRest.exe" -namespace FormsMvvmcross -Input http://localhost:10739/swagger/v1/swagger.json -o "%CD%\Generated"
	echo swagger files have been sucessfully generated
)
:END