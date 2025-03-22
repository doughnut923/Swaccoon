using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SwacoonNarrative
{


    /// <summary>
    /// An inline struct to contain dialogue activation conditions
    /// </summary>
    [System.Serializable]
    public struct Condition
    {
        [Tooltip("Which flag to check")]
        public string flagID;

        [Tooltip("If the flag matches this value this condition will succeed.")]
        public bool expectedValue;
    }

    /// <summary>
    /// Component that controls the sequencing of a dialogue.
    /// </summary>
    public class SwacoonDialogueSequencer : MonoBehaviour
    {
        //Event Callbacks
        public UnityEvent onStarted;
        public UnityEvent onFinish;

        //Sub-Object references
        [SerializeField] private SwacoonDialogueBox textbox;
        [SerializeField] private SwacoonDialoguePortraits portraits;
        [SerializeField] private SwacoonDialogueChoices choices;

        //Object properties
        private SwacoonDialogueSequence currentDialog;  //set when we are playing
        private int currentLine = 0;  //current line in the sequence we are playing
        private bool isPlaying = false;


        /// <summary>
        /// Starts playing the given sequence
        /// </summary>
        /// <param name="dialogue">Dialogue resource to play</param>
        public void PlaySequence(SwacoonDialogueSequence dialogue)
        {
            //Debug.Log("wohoo in the sequecer");
            if (dialogue.IsEmpty())
            {
                Debug.LogWarning("Playing sequence stopped because DialogueSequence was empty");
                return;
            }

            //Set our state
            currentDialog = dialogue;
            currentLine = 0;
            isPlaying = true;
            //Debug.Log("is it playing" + isPlaying);
            //Open and play
            textbox.OpenTextbox();
            //Debug.Log("shoule have gone into Opentextbox");
            ParseLine(currentLine);
            //Debug.Log(ParseLine(currentLine));
            onStarted.Invoke();
        }

        /// <summary>
        /// Event reciever that advances the dialogue to the next line or closes if the dialogue is finished
        /// </summary>
        public void onSequenceAdvanced()
        {
            
            bool hasNext = currentDialog.HasLine(currentLine + 1);
            //Debug.Log("is there another line " + hasNext);


            if (hasNext)
            {
                //Start next line
                currentLine++;
                ParseLine(currentLine);
            }
            else
            {
                //Finished, close textbox
                //onFinish.Invoke();
                portraits.ClosePortraits();
                textbox.CloseTextbox();
                onFinish.Invoke();
            }
        }

        /// <summary>
        /// Applies the current line.
        /// </summary>
        /// <param name="lineNum">Line number</param>
        private void ParseLine(int lineNum)
        {
            //Debug.Log("dialog currently playing " + currentDialog);
            //Debug.Log("parsing the lines");
            //Apply to textbox
            textbox.SetLine(currentDialog.GetRowDialogue(lineNum));
            //Debug.Log("hahaha " + textbox);
            //Apply to textbox speaker name
            string name = currentDialog.GetRowName(lineNum);
            if (name != "")//Only apply if not empty
            {
                textbox.SetName(name);
                //Debug.Log("name is "+name);
            }
            //Apply to left portrait
            string portraitLeft = currentDialog.GetRowPortraitLeft(lineNum);

            if (portraitLeft != "")//Only apply if not empty
            {
                Sprite spr = SwacoonDialoguePortraitContainer.GetPortrait(portraitLeft);
                portraits.SetPortraitSpriteLeft(spr);
            }
            else{
                portraits.ClosePortraitLeft();
            }

            //Apply to right portrait
            string portraitRight = currentDialog.GetRowPortraitRight(lineNum);
            if (portraitRight != "")//Only apply if not empty
            {
                Sprite spr = SwacoonDialoguePortraitContainer.GetPortrait(portraitRight);
                portraits.SetPortraitSpriteRight(spr);
            }
            else{
                portraits.ClosePortraitRight();
            }
            //Debug.Log("got all portraits");

            //Play sound if any
            string soundClip = currentDialog.GetRowSoundClip(lineNum);
            if(soundClip!= "")//Only apply if not empty
            {
                AudioClip clip = SwacoonDialogueSounds.GetSound(soundClip);
                SwacoonDialogueSounds.AudioSource.PlayOneShot(clip);
            }
            //Debug.Log("done parsing");

        }

        /// <summary>
        /// Checks if the sequencer is playing a sequence
        /// </summary>
        public bool IsPlaying()
        {
            //Debug.Log("writing the text!");
            return textbox.IsActive;
        }

    }
}