namespace OrigamiFSharp

open UnityEngine

type SphereCommands() =
    inherit MonoBehaviour()

    let mutable maybeOriginalPosition: option<Vector3> = None

    // Grab the original local position of the sphere when the app starts.
    let setOriginalPosition (this: SphereCommands) = 
        let position = this.transform.localPosition
        maybeOriginalPosition <- Some(position)
        position
        
    let getOriginalPosition(this: SphereCommands) = 
        match maybeOriginalPosition with
            | Some(position) -> position
            | None -> this |> setOriginalPosition

    member this.Start() =
        this |> setOriginalPosition

    // Called by GazeGestureManager when the user performs a Select gesture
    member this.OnSelect() =
        // If the sphere has no Rigidbody component, add one to enable physics.
        match this.GetComponent<Rigidbody>() with
            | null ->
                let rigidbody = this.gameObject.AddComponent<Rigidbody>()
                rigidbody.collisionDetectionMode <- CollisionDetectionMode.Continuous
            | _ -> ()


    member this.OnReset() =
        // If the sphere has a Rigidbody component, remove it to disable physics.
        match this.GetComponent<Rigidbody>() with
            | null -> ()
            | rigidbody -> Object.DestroyImmediate(rigidbody)

        // Put the sphere back into its original local position.
        this.transform.localPosition <- this |> getOriginalPosition 

    // Called by SpeechManager when the user says the "Drop sphere" command
    member this.OnDrop() =
        // Just do the same logic as a Select gesture.
        this.OnSelect()
