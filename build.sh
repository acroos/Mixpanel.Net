#!/bin/sh

# Update nuget exe
rm -rf tools/NuGet
mkdir tools/NuGet
curl -o tools/NuGet/nuget.exe https://dist.nuget.org/win-x86-commandline/latest/nuget.exe

# restore packages
mono tools/NuGet/nuget.exe install tools/packages.config -ExcludeVersion -OutputDirectory tools

# run FAKE script
mono tools/FAKE/tools/FAKE.exe build.fsx
