namespace OrigamiFSharp

open UnityEngine
open UnityEngine.VR.WSA.Input

type GazeGestureManager() =
    inherit MonoBehaviour()

    // Static fields are not serialized in Unity, but this will last as long as the class is loaded
    static let mutable instance: Option<GazeGestureManager> = None

    // We need to serialize the recognizer. Otherwise when the manager is deserialized
    // it will have a null recognizer
    [<SerializeField>] [<DefaultValue>] val mutable recognizer: GestureRecognizer
   
    // Represents the hologram that is currently being gazed at.
    let mutable maybeFocusedObject: Option<GameObject> = None

    member this.Start() =
        instance <- Some(this)

        // Set up a GestureRecognizer to detect Select gestures.
        this.recognizer <- new GestureRecognizer()
        this.recognizer.add_TappedEvent(
            fun source tapCount ray ->
                // Send an OnSelect message to the focused object and its ancestors.
                match maybeFocusedObject with
                    | Some(focusedObject) -> focusedObject.SendMessageUpwards("OnSelect")
                    | None -> ()
            )
        this.recognizer.StartCapturingGestures()

    // Update is called once per frame
    member this.Update() =
        // Do a raycast into the world based on the user's head position and orientation.
        let headPosition = Camera.main.transform.position
        let gazeDirection = Camera.main.transform.forward

        let maybeNewFocusedObject = match Physics.Raycast(headPosition, gazeDirection) : (bool * RaycastHit) with
            // If the raycast hit a hologram, use that as the focused object.
            | true, hitInfo -> hitInfo.collider.gameObject |> Some
            // If the raycast did not hit a hologram, clear the focused object. The outside world expects null, so we have to use it.
            | _ -> None

        // If the focused object changed this frame, start detecting fresh gestures again.
        if maybeNewFocusedObject <> maybeFocusedObject then
            maybeFocusedObject <- maybeNewFocusedObject
            this.recognizer.CancelGestures()
            this.recognizer.StartCapturingGestures()