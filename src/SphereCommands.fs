namespace OrigamiFSharp

open UnityEngine

type SphereCommands() =
    inherit MonoBehaviour()

    // Called by GazeGestureManager when the user performs a Select gesture
    member this.OnSelect() =
        // If the sphere has no Rigidbody component, add one to enable physics.
        match this.GetComponent<Rigidbody>() with
            | null ->
                let rigidbody = this.gameObject.AddComponent<Rigidbody>()
                rigidbody.collisionDetectionMode <- CollisionDetectionMode.Continuous
            | _ -> ()