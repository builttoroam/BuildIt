/************************************************/
/**       Creating NuGet package .nupkg        **/
/************************************************/

1. Create .nuspec file (skip if .nuspec file already exist)
 1a. Ensure that NuGet.exe is in your PATH
  1aa. Download NuGet.exe
  1ab. Add NuGet.exe to the PATH
 1b. Run command prompt
 1c. Execute command on your assembly 'nuget spec <AssemblyName>.dll' which will generate .nuspec file
2. Move .nuspec file  to the directory with .csproj file
3. Run command prompt
4. Navigate to the directory with .csproj and .nuspec file
 4a. Ensure that NuGet.exe is in your PATH
5. Execute command 'nuget pack SQLiteDatabaseWrapper.csproj -prop configuration=release'
6. Package .nupkg should be created
 6a. Download NuGet Package Explorer
 6b. Ensure all that is correct (change if necessary)
7. Publish NuGet package
 7a. Create account at NuGet.org
 7b. Run command prompt
 7c. Set API key by executing command 'nuget setApiKey <key>'
 7d. Publish package by executing command 'nuget push <packageName>.nupkg'
8. DONE

HELP --> https://docs.nuget.org/create/creating-and-publishing-a-package
 