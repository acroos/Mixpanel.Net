#r @"tools/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let buildDir = "./build/"
let testDir = "./test/"

let nugetBuildDir = buildDir
let nugetDistDir = (buildDir + "nuget/")

// Targets
Target "Clean" (fun _ ->
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
  !! (testDir + "*.dll")
    |> NUnit (fun p ->
      {p with
        OutputFile = "TestResults.xml"
        DisableShadowCopy = true})
)

// Dependencies
"Clean"
  ==> "BuildLibrary"
  ==> "BuildTest"
  ==> "UnitTests"

// Start
RunTargetOrDefault "UnitTests"
