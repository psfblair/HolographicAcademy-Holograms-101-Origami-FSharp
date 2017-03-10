namespace OrigamiFSharp

open UnityEngine

type SphereSounds () =
    inherit MonoBehaviour ()

    let mutable maybeAudioSource: option<AudioSource>  = None
    let mutable maybeImpactClip: option<AudioClip> = None
    let mutable maybeRollingClip: option<AudioClip> = None
    let mutable rolling: bool = false

    // Grab the original local position of the sphere when the app starts.
    let setAudioSource (this: SphereSounds) = 
        let source = this.gameObject.AddComponent<AudioSource>()
        source.playOnAwake <- false
        source.spatialize <- true
        source.spatialBlend <- 1.0f
        source.dopplerLevel <- 0.0f
        source.rolloffMode <- AudioRolloffMode.Logarithmic
        source.maxDistance <- 20.0f
        maybeAudioSource <- Some(source)
        source
        
    let getAudioSource (this: SphereSounds) = 
        match maybeAudioSource with
            | Some(source) -> source
            | None -> this |> setAudioSource

    let setImpactClip (this: SphereSounds) = 
        let clip = Resources.Load<AudioClip>("Impact")
        maybeImpactClip <- Some(clip)
        clip

    let getImpactClip (this: SphereSounds) = 
        match maybeImpactClip with
            | Some(clip) -> clip
            | None -> this |> setImpactClip

    let setRollingClip (this: SphereSounds) = 
        let clip = Resources.Load<AudioClip>("Rolling")
        maybeRollingClip <- Some(clip)
        clip

    let getRollingClip (this: SphereSounds) = 
        match maybeRollingClip with
            | Some(clip) -> clip
            | None -> this |> setRollingClip

    member this.Start() =
        // Add an AudioSource component and set up some defaults
        this |> setAudioSource |> ignore
               
        // Load the Sphere sounds from the Resources folder
        this |> setImpactClip |> ignore
        this |> setRollingClip |> ignore

    // Occurs when this object starts colliding with another object
    member this.OnCollisionEnter(collision: Collision) =
        // Play an impact sound if the sphere impacts strongly enough.
        if collision.relativeVelocity.magnitude >= 0.1f then
            let audioSource = this |> getAudioSource
            audioSource.clip <- this |> getImpactClip
            audioSource.Play()

    // Occurs each frame that this object continues to collide with another object
    member this.OnCollisionStay(collision: Collision) =
        let rigid = this.gameObject.GetComponent<Rigidbody>()
        let audioSource = this |> getAudioSource

        // Play a rolling sound if the sphere is rolling fast enough.
        if not rolling && rigid.velocity.magnitude >= 0.01f then
            rolling <- true
            audioSource.clip <- this |> getRollingClip
            audioSource.Play()
        // Stop the rolling sound if rolling slows down.
        else if rolling && rigid.velocity.magnitude < 0.01f then
            rolling <- false
            audioSource.Stop()

    // Occurs when this object stops colliding with another object
    member this.OnCollisionExit(collision: Collision) =
        // Stop the rolling sound if the object falls off and stops colliding.
        if rolling then
            rolling <- false
            let audioSource = this |> getAudioSource
            audioSource.Stop()
