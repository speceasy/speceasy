SET MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe

%MSBUILD% ..\SpecEasy.sln /target:Rebuild /p:Configuration="Release 4.5"
%MSBUILD% ..\SpecEasy.sln /target:Rebuild /p:Configuration="Release 4.5.1"

..\.nuget\nuget pack ..\SpecEasy\SpecEasy.csproj -Prop Configuration="Release 4.5.1" -Verbosity detailed
