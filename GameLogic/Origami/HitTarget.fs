namespace OrigamiFSharp

open UnityEngine

type HitTarget() =
    inherit MonoBehaviour()

    // These public fields become settable properties in the Unity editor.
    [<SerializeField>] [<DefaultValue>] val mutable Underworld: GameObject
    [<SerializeField>] [<DefaultValue>] val mutable ObjectToHide: GameObject

    // Occurs when this object starts colliding with another object
    member this.OnCollisionEnter(collision: Collision) =
        // Hide the stage and show the underworld.
        this.ObjectToHide.SetActive(false)
        this.Underworld.SetActive(true)

        // Disable Spatial Mapping to let the spheres enter the underworld.
        SpatialMapping.Instance.MappingEnabled <- false
