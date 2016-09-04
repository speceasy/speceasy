# Deploy a NuGet Release

1. Update AssemblyInfo.cs file attributes:
  - `AssemblyCopyright`
  - `AssemblyVersion`
  - other attributes, if necessary.
1. Update the `SpecEasy.nuspec` file:
  - `releaseNotes`
  - `copyright`
  - other elements as required.
1. Update `build/build.nuget.bat` with new builds and/or version numbers, if necessary.
1. Build NuGet package and deploy:
  1. Open a command prompt in the `build` folder.
  1. Run `build.nuget.bat`. This will create the NuGet package - `SpecEasy.x.x.x.x.nupkg`.
  1. Push to nuget.org.

    ..\.nuget\NuGet.exe push SpecEasy.x.x.x.x.nupkg -s http://www.nuget.org/packages/SpecEasy [api key]

