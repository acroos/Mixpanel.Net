#r @"tools/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let buildDir = "./build/"
let testDir = "./test/"

let nugetBuildDir = buildDir
let nugetDistDir = (buildDir + "nuget/")

let releaseNotesDir = "./release-notes/"

let testResultsFile = "TestResults.xml"
let versionFile = "version.txt"

// Targets
Target "Clean" (fun _ ->
  DeleteFile testResultsFile
  CleanDirs [buildDir; testDir]
)

Target "BuildLibrary" (fun _ ->
  !! "src/Mixpanel.NET.PCL/**.csproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "BuildLibrary-Output: "
)

Target "BuildTest" (fun _ ->
  !! "src/Mixpanel.NET.PCL.UnitTests/**.csproj"
    |> MSBuildRelease testDir "Build"
    |> Log "BuildTest-Output: "
)

Target "UnitTests" (fun _ ->
  !! (testDir + "Mixpanel.NET.PCL.UnitTests.dll")
    |> NUnit (fun p ->
      {p with
        OutputFile = testResultsFile
        DisableShadowCopy = true})
)

Target "CreatePackage" (fun _ ->
  let versionText = ReadFileAsString versionFile
  let version = trim versionText
  let releaseNotesFile = releaseNotesDir + version + ".md"
  let releaseNotes = ReadFileAsString releaseNotesFile

  CreateDir nugetBuildDir
  CreateDir nugetDistDir
  NuGet (fun p ->
    {p with
      Version = version
      ReleaseNotes = releaseNotes
      OutputPath = nugetDistDir
      WorkingDir = nugetBuildDir})
      "Mixpanel.NET.nuspec"
)

// Dependencies
"Clean"
  ==> "BuildLibrary"
  ==> "BuildTest"
  ==> "UnitTests"
  ==> "CreatePackage"

// Start
RunTargetOrDefault "CreatePackage"
