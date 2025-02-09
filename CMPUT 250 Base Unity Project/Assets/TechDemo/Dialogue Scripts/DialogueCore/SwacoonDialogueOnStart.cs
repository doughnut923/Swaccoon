using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwacoonNarrative
{
    public class SwacoonDialogueOnStart : MonoBehaviour
    {

        /// <summary> The csv file containing the dialogue to be played. </summary>
        [SerializeField] private TextAsset dialogueCSV;

        [Header("Set Dialogue Flag After Finishing Dialogue")]
        [Tooltip("Which flag to assign after finishing this dialogue.")]
        [SerializeField]
        private string writeToFlagId = "";
        [SerializeField] private bool writeToFlagValue = false;

        [Header("Conditions")]
        [SerializeField] private List<Condition> conditions = new List<Condition>();

        // Start is called before the first frame update
        void Start()
        {
            if (dialogueCSV!=null){//If we have a dialogue
                //Debug.Log("entering the on start dialgue");
                SwacoonDialogueSystem.OnDialogueEnd.AddListener(OnDialogueEnd);
                SwacoonDialogueSystem.PlaySequence(dialogueCSV);
            }
        }

        /// <summary>
        /// Callback reciever for ehrn dialogue ends.
        /// Writes to flags if it is set
        /// </summary>
        private void OnDialogueEnd()
        {
            //Debug.Log("entering the on end dialgue");
            if (writeToFlagId != "")
            {
                SwacoonDialogueFlags.SetFlag(writeToFlagId, writeToFlagValue);
            }
            SwacoonDialogueSystem.OnDialogueEnd.RemoveListener(OnDialogueEnd);//We shouldn't recieve this if we aren't playing something.
            Destroy(this);
        }
    }
}
