// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"


// Filesets
let appReferences  =
    !! "/**/*.csproj"
    ++ "/**/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
    |> Log "AppBuild-Output: "
)

let remoteProjectPath = "/Users/paulblair/Documents/workspaces-Unity/remote/psfblair/Documents/holodevelop/MicrosoftAcademy/101-Origami"

Target "Deploy" (fun _ ->
    let origamiDll = buildDir + "Origami.dll"
    let fsharpDll = buildDir + "FSharp.Core.dll"
    FileHelper.Copy "../Assets/Plugins" [ origamiDll; fsharpDll ]
    FileHelper.Copy (remoteProjectPath + "/Assets/Plugins") [ origamiDll; fsharpDll ]
)

// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Deploy"
