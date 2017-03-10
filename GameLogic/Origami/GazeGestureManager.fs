namespace OrigamiFSharp

open UnityEngine
open UnityEngine.VR.WSA.Input

type GazeGestureManager() =
    inherit MonoBehaviour()

    // Static fields are not serialized in Unity, but this will last as long as the class is loaded
    static let mutable instance: Option<GazeGestureManager> = None

    // Represents the hologram that is currently being gazed at.
    let mutable maybeFocusedObject: Option<GameObject> = None

    let mutable maybeRecognizer: Option<GestureRecognizer> = None

    let setGestureRecognizer = 
        // Set up a GestureRecognizer to detect Select gestures.
        let recognizer = new GestureRecognizer()
        recognizer.add_TappedEvent(
            fun source tapCount ray ->
                // Send an OnSelect message to the focused object and its ancestors.
                match maybeFocusedObject with
                    | Some(focusedObject) -> focusedObject.SendMessageUpwards("OnSelect")
                    | None -> ()
        )
        recognizer.StartCapturingGestures()
        maybeRecognizer <- Some(recognizer)
        recognizer
        
    let resetRecognizer() = 
        let recognizer = 
            match maybeRecognizer with
                | Some(recognzr) -> recognzr
                | None -> setGestureRecognizer
        recognizer.CancelGestures()
        recognizer.StartCapturingGestures()

    static member Instance with get () = instance 

    member this.FocusedObject with get () = maybeFocusedObject

    member this.Start() =
        instance <- Some(this)
        setGestureRecognizer

    // Update is called once per frame
    member this.Update() =
        // Do a raycast into the world based on the user's head position and orientation.
        let headPosition = Camera.main.transform.position
        let gazeDirection = Camera.main.transform.forward

        let maybeNewFocusedObject = 
            match Physics.Raycast(headPosition, gazeDirection) : (bool * RaycastHit) with
                // If the raycast hit a hologram, use that as the focused object.
                | true, hitInfo -> hitInfo.collider.gameObject |> Some
                // If the raycast did not hit a hologram, clear the focused object. The outside world expects null, so we have to use it.
                | _ -> None

        // If the focused object changed this frame, start detecting fresh gestures again.
        if maybeNewFocusedObject <> maybeFocusedObject then
            maybeFocusedObject <- maybeNewFocusedObject
            resetRecognizer()
