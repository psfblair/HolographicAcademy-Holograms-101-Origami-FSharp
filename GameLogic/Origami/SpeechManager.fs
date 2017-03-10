namespace OrigamiFSharp

open UnityEngine
open UnityEngine.Windows.Speech

open System.Collections.Generic
//for extension methods on KeyCollection to allow ToArray to be called on it
open System.Linq

type SpeechManager() =
    inherit MonoBehaviour()

    let mutable keywordRecognizer: Option<KeywordRecognizer> = None

    let keywords = Dictionary<string, unit -> unit>()

    let keywordRecognizer_OnPhraseRecognized (args: PhraseRecognizedEventArgs) =
        match keywords.TryGetValue(args.text) with
            | (true, keywordAction) -> keywordAction ()
            | _ -> ()

    member this.Start() =
        keywords.Add("Reset world", fun () ->
            // Call the OnReset method on every descendant object.
            this.BroadcastMessage("OnReset")
        )

        keywords.Add("Drop Sphere", fun () ->
            match GazeGestureManager.Instance with
            | None -> ()
            | Some(gazeMgr) -> 
                match gazeMgr.FocusedObject with
                | None -> ()
                | Some(focusObject) ->
                    // Call the OnDrop method on just the focused object.
                    focusObject.SendMessage("OnDrop")
        )

        // Tell the KeywordRecognizer about our keywords.
        let recognizer =  new KeywordRecognizer(keywords.Keys.ToArray())
        keywordRecognizer <- Some(recognizer)

        // Register a callback for the KeywordRecognizer and start recognizing!
        recognizer.add_OnPhraseRecognized (fun eventArgs -> keywordRecognizer_OnPhraseRecognized eventArgs)
        recognizer.Start()
