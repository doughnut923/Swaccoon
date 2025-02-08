using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SwacoonNarrative
{

    
    /// <summary>
    /// The core dialogue system intterface for objects in the scene.
    /// </summary>
    public class SwacoonDialogueSystem : MonoBehaviour
    {
        //Event Callbacks
        public UnityEvent onDialogueStarted;
        public UnityEvent onDialogueEnd;
        public static UnityEvent OnDialogueEnd { get { return Instance.onDialogueEnd; } }

        //Singleton pattern
        private static SwacoonDialogueSystem _instance;
        public static SwacoonDialogueSystem Instance { get { return _instance; } }


        [SerializeField] private SwacoonDialogueSequencer dialogueSequencer;//reference to the sequencer
        [SerializeField] private Canvas dialogueCanvas; //reference to the ui canvas

        public SwacoonDialogueSystem()
        {
            _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            //Add event listeners
            dialogueSequencer.onFinish.AddListener(OnFinish);
            dialogueCanvas.enabled = true;//Enable the canvas only on runtime so it doesn't get in the way of scene editing
        }


        /// <summary>
        /// Plays the dialogue sequence.
        /// </summary>
        /// <param name="asset">Text asset source to play</param>
        public static void PlaySequence(TextAsset asset)
        {
            Debug.Log("in the first play sequence");
            PlaySequence(new SwacoonDialogueSequence(asset));
        }

        /// <summary>
        /// Plays the dialogue sequence.
        /// </summary>
        /// <param name="dialogue">Dialogue sequence object to play</param>
        public static void PlaySequence(SwacoonDialogueSequence dialogue)
        {
            Debug.Log("yay playing sequence now");
            Instance.dialogueSequencer.PlaySequence(dialogue);
            Debug.Log("invoke next");
            Instance.onDialogueStarted.Invoke();
            Debug.Log("has been invoked");
        }

        /// <summary>
        /// Checks if the sequencer is currently playing something
        /// </summary>
        public static bool IsPlaying()
        {
            Debug.Log("is it playing?");
            return Instance.dialogueSequencer.IsPlaying();
        }

        /// <summary>
        /// Callback reciever for when the sequence has ended.
        /// </summary>
        private static void OnFinish()
        {
            Debug.Log("done the sequence");
            Instance.onDialogueEnd.Invoke();
            //IsPlaying = false;
            //SwacoonDialogueBox.IsActive = false;
            Debug.Log("are we still playing "+SwacoonDialogueSystem.IsPlaying());
        }
    }
}