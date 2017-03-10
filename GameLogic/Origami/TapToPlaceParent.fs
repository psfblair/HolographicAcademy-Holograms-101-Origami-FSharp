namespace OrigamiFSharp

open UnityEngine

type TapToPlaceParent() =
    inherit MonoBehaviour ()

    let mutable placing: bool = false

    // Called by GazeGestureManager when the user performs a Select gesture
    member this.OnSelect() =
        // On each Select gesture, toggle whether the user is in placing mode.
        placing <- not placing

        // If the user is in placing mode, display the spatial mapping mesh.
        if placing then
            SpatialMapping.Instance.DrawVisualMeshes <- true
        // If the user is not in placing mode, hide the spatial mapping mesh.
        else
            SpatialMapping.Instance.DrawVisualMeshes <- false

    // Update is called once per frame
    member this.Update() =
        // If the user is in placing mode,
        // update the placement to match the user's gaze.
        if placing then
            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            let headPosition = Camera.main.transform.position
            let gazeDirection = Camera.main.transform.forward
            let maxDistance = 30.0f

            // GOTCHA I don't think F# can tuple the out parameter when it's the third parameter of five. Or maybe the
            // method signature becomes ambiguous. In any case, we have to handle the out parameter explicitly here.
            let mutable hitInfo = Unchecked.defaultof<RaycastHit>

            if Physics.Raycast(headPosition, gazeDirection, &hitInfo, maxDistance, SpatialMapping.PhysicsRaycastMask) then
                // Move this object's parent object to where the raycast hit the Spatial Mapping mesh.
                this.transform.parent.position <- hitInfo.point
                // Rotate this object's parent object to face the user.
                // GOTCHA In order to mutate a mutable field of a struct, the struct has to be mutable too.
                let mutable toQuat: Quaternion = Camera.main.transform.localRotation
                toQuat.x <- 0.0f
                toQuat.z <- 0.0f
                this.transform.parent.rotation <- toQuat

// GOTCHA - "the namespace or module SpatialMapping is not defined"
// to get SpatialMapping module have to add Assembly-CSharp to solution

// If SpatialMapping still doesn't resolve
// 1. Check that both projects target Mono/.NET 3.5
// 2. If you're using Xamarin Studio, try closing/reopenint it.
