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

let sshDestination = "psfblair@holodevelop:/C:/Users/psfblair/Documents/holodevelop/MicrosoftAcademy/101-Origami/Assets/Plugins/"
let privateKeyPath = "~/.ssh/holodevelop"

let copyToRemoteProject localDll = 
    SCP (fun p -> { p with PrivateKeyPath = privateKeyPath; ToolPath = "scpq"}) localDll sshDestination

Target "Deploy" (fun _ ->
    let origamiDll = buildDir + "Origami.dll"
    let fsharpDll = buildDir + "FSharp.Core.dll"
    Copy "../Assets/Plugins" [ origamiDll; fsharpDll ]
    try
        copyToRemoteProject origamiDll
        copyToRemoteProject fsharpDll
    with
        | _ -> eprintfn "Unable to copy to remote host."
)

// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Deploy"
