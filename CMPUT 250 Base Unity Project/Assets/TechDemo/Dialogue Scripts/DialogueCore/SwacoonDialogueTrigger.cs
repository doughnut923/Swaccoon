using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwacoonNarrative
{
    /// <summary>
    /// A script that will activate a dialogue when triggered.
    /// Must be triggered manually but it will attempt to automatically connect to a ClickableObject script.
    /// </summary>
    public class SwacoonDialogueTrigger : MonoBehaviour
    {
        public bool repeatable = false;//whether or not this dialogue can repeat

        /// <summary> The csv file containing the dialogue to be played. </summary>
        [SerializeField] private TextAsset dialogueCSV;

        [Header("Conditions")]
        [SerializeField] private List<Condition> conditions = new List<Condition>();


        [Header("Set Dialogue Flag After Finishing Dialogue")]
        [Tooltip("Which flag to assign after finishing this dialogue.")]
        [SerializeField]
        private string writeToFlagId = "";
        [SerializeField] private bool writeToFlagValue = false;

        public bool isDialogueDone = false;


        /// <summary>
        /// Call this to activate the dialogue. If condition are set they must all be satisfied.
        /// </summary>
        public void Trigger()
        {
            Debug.Log("in swacoondialoguetrigger");
            isDialogueDone = false;
            PlayerManager.Instance.CurrentCharacter.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.CUTSCENE_PLAYING;
            PlayerManager.Instance.CurrentCharacter.GetComponent<PlayerBehaviour>().movement = Vector2.zero;
            PlayerManager._playerManagerState = PlayerManagerState.CUTSCENE_PLAYING;
            //Debug.Log("trigger() called");
            //Debug.Log("smothing is already playing "+ SwacoonDialogueSystem.IsPlaying());
            //Debug.Log("trigger " + SwacoonDialogueSystem.IsPlaying());
            if (SwacoonDialogueSystem.IsPlaying())
            {
                //Debug.Log("something already playing");
                return;//Don't activate if already playing something
            }


            //Check conditions
            if (!AreConditionsTrue())
            {
                return; //Cancel activation if any conditions fail
            }

            //Activate Dialogue
            //Debug.Log("entering play sequence");
            
            SwacoonDialogueSystem.OnDialogueEnd.AddListener(OnDialogueEnd);
            SwacoonDialogueSystem.PlaySequence(dialogueCSV);
        }

        /// <summary>
        /// Evaluates whether all conditions are satisfied.
        /// </summary>
        private bool AreConditionsTrue()
        {
            foreach (Condition condition in conditions)
            {
                if (SwacoonDialogueFlags.GetFlagValue(condition.flagID) != condition.expectedValue)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Callback reciever for when dialogue ends.
        /// Writes to flags if it is set
        /// </summary>
        private void OnDialogueEnd()
        {
            isDialogueDone = true;
            if (writeToFlagId != "")
            {
                SwacoonDialogueFlags.SetFlag(writeToFlagId, writeToFlagValue);
            }
            SwacoonDialogueSystem.OnDialogueEnd.RemoveListener(OnDialogueEnd);//We shouldn't recieve this if we aren't playing something.
            PlayerManager.Instance.CurrentCharacter.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.IDLE;
            PlayerManager._playerManagerState = PlayerManagerState.NOT_SWAPPING;
            if (!repeatable){
                Destroy(this);
            }
        }
    }
}