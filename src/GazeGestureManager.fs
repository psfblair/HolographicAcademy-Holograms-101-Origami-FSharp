namespace OrigamiFSharp

open UnityEngine
open UnityEngine.VR.WSA.Input

type GazeGestureManager() =
    inherit MonoBehaviour()

    // All this rigmarole is needed because we need the initial value on the property to be null
    // Static fields are not serialized in Unity
    [<DefaultValue>] static val mutable private instance: GazeGestureManager
    static member Instance = GazeGestureManager.instance
    
    // Represents the hologram that is currently being gazed at.
    [<SerializeField>] [<DefaultValue>] val mutable focusedObject: GameObject
    member this.FocusedObject = this.focusedObject
    
    [<SerializeField>] [<DefaultValue>] val mutable recognizer: GestureRecognizer

    // Use this for initialization
    member this.Start() =
        GazeGestureManager.instance <- this

        // Set up a GestureRecognizer to detect Select gestures.
        this.recognizer <- new GestureRecognizer()
        this.recognizer.add_TappedEvent(
            fun source tapCount ray ->
                // Send an OnSelect message to the focused object and its ancestors.
                match this.FocusedObject with
                    | null -> ()
                    | _ -> this.FocusedObject.SendMessageUpwards("OnSelect")
            )
        this.recognizer.StartCapturingGestures()

    // Update is called once per frame
    member this.Update() =
        // Figure out which hologram is focused this frame.
        let oldFocusObject: GameObject = this.FocusedObject

        // Do a raycast into the world based on the user's head position and orientation.
        let headPosition = Camera.main.transform.position
        let gazeDirection = Camera.main.transform.forward

        match Physics.Raycast(headPosition, gazeDirection) : (bool * RaycastHit) with
            // If the raycast hit a hologram, use that as the focused object.
            | true, hitInfo -> this.focusedObject <- hitInfo.collider.gameObject
            // If the raycast did not hit a hologram, clear the focused object. The outside world expects null, so we have to use it.
            | _ -> this.focusedObject <- null

        // If the focused object changed this frame, start detecting fresh gestures again.
        if this.FocusedObject <> oldFocusObject then
            this.recognizer.CancelGestures()
            this.recognizer.StartCapturingGestures()