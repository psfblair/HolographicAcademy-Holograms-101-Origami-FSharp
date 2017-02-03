namespace OrigamiFSharp

open UnityEngine

type WorldCursor() =
    inherit MonoBehaviour()

    [<SerializeField>] [<DefaultValue>] val mutable meshRenderer: MeshRenderer

        // Use this for initialization
        member this.Start() =
            // On start, grab the mesh renderer that's on the same object as this script.
            this.meshRenderer <- this.gameObject.GetComponentInChildren()

        // Update is called once per frame
        member this.Update() =
            // Do a raycast into the world based on the user's
            // head position and orientation.
            let headPosition = Camera.main.transform.position;
            let gazeDirection = Camera.main.transform.forward;

            match Physics.Raycast(headPosition, gazeDirection) : (bool * RaycastHit) with
                | true, hitInfo ->
                    // If the raycast hit a hologram...
                    // Display the cursor mesh.
                    this.meshRenderer.enabled <- true

                    // Move thecursor to the point where the raycast hit.
                    this.transform.position <- hitInfo.point

                    // Rotate the cursor to hug the surface of the hologram.
                    this.transform.rotation <- Quaternion.FromToRotation(Vector3.up, hitInfo.normal)
                | _ ->
                    // If the raycast did not hit a hologram, hide the cursor mesh.
                    this.meshRenderer.enabled <- false
