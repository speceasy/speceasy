REM Change path to msbuild if not in system PATH.
SET MSBUILD="MSBuild.exe"

%MSBUILD% ..\SpecEasy.sln /target:Rebuild /p:Configuration="Release 4.5"
%MSBUILD% ..\SpecEasy.sln /target:Rebuild /p:Configuration="Release 4.5.1"

..\.nuget\nuget pack ..\SpecEasy\SpecEasy.csproj -Prop Configuration="Release 4.5.1" -Verbosity detailed
