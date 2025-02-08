using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SwacoonNarrative
{
    /// <summary>
    /// A component class for the dialogue textbox display.
    /// </summary>
    public class SwacoonDialogueBox : MonoBehaviour
    {
        //Event Callbacks
        public UnityEvent onAdvance;

        //Sub-object references
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private SwacoonAdvanceArrow advanceArrow;
        [SerializeField] private Animator animator;

        //Object properties
        public float charactersPerSecond = 45.0f;//how many characters are displayed per second
        private float currentCharacter = 0f; //current position where
        private int textLength = 0;//cached content length

        //State tracking variables and getters, useful for timing
        //serialized so the animator can edit them but shouldn't be modified otherwise
        [SerializeField] [HideInInspector] private bool isOpen = false;
        [SerializeField] [HideInInspector] private bool isActive = false;
        [SerializeField] [HideInInspector] private bool isSpedUp = false;
        public bool IsOpen { get { return isOpen; } }
        public bool IsActive { get { return isActive; } }

        // Start is called before the first frame update
        private void Start()
        {
            //Connect onAdvance event to hide the advancearrow object on invoke
            onAdvance.AddListener(() => advanceArrow.SetVisible(false));
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log("is it open " + isOpen);
            if (isOpen)
            {
                //Update state
                CheckInput();
                if (!isSpedUp)
                {
                    UpdateText();
                }
                
            }
        }

        /// <summary>
        /// Checks player input during frame update
        /// </summary>
        private void CheckInput()
        {
            Debug.Log("in checkinput");
            //Input for advancing textbox
            if (Input.GetButtonDown("Submit"))
            {
                Debug.Log("mouse ");
                if (isEndOfText() || isSpedUp==true)
                {
                    AdvanceLine();
                    isSpedUp = false;
                }
            }
        }

        /// <summary>
        /// Advances per character text.
        /// </summary>
        private void UpdateText()
        {
            Debug.Log("in the updatetext, is currentcharacter < textlength? "+(currentCharacter<textLength));
            Debug.Log("textlength is "+textLength);
            if (currentCharacter < textLength)
            {
                Debug.Log("writing the text!");
                //Advance visible characters
                if (Input.GetMouseButtonDown(1))
                {

                    Debug.Log("mouse has been pressed");
                    //currentCharacter += Time.deltaTime * spedUpCharactersPerSecond;
                    textLabel.maxVisibleCharacters = textLength;
                    //isOpen = false;
                    isSpedUp = true;
                    advanceArrow.SetVisible(true);
                    CheckInput();
                    
                    //if (isEndOfText()) {
                    //    isSpedUp = true;
                    //    advanceArrow.SetVisible(true);

                    //}
                }
                else
                {
                    Debug.Log("current character speed " + currentCharacter);
                    currentCharacter += Time.deltaTime * charactersPerSecond;
                    textLabel.maxVisibleCharacters = Mathf.FloorToInt(currentCharacter);
                }
                

                if (isEndOfText())
                {
                    Debug.Log("end of current text");
                    advanceArrow.SetVisible(true);
                }
            }
        }

        /// <summary>
        /// Opens the textbox with playing the open animation
        /// </summary>
        public void OpenTextbox()
        {
            Debug.Log("opeing the textbox "+isOpen);

            animator.SetBool("isOpen", true);
            advanceArrow.SetVisible(false);
            nameLabel.text = "";
            isActive = true;
            isOpen = true;
            Debug.Log("opentextbox, isopen" + isOpen);
        }

        /// <summary>
        /// Closes the textbox by playing the close animation
        /// </summary>
        public void CloseTextbox()
        {
            Debug.Log("closing the textbox");
            animator.SetBool("isOpen", false);
            isActive = false;
        }

        /// <summary>
        /// Sets the line and resets the per character scrolling
        /// </summary>
        /// <param name="sourceText"></param>
        public void SetLine(string sourceText)
        {
            Debug.Log("setting the line");
            Debug.Log("line is " + sourceText);
            //textLabel.SetText("Hello World");
            Debug.Log("max visible characters is " + textLabel.maxVisibleCharacters);
            Debug.Log("current character is " + currentCharacter);
            Debug.Log("text length is " + textLength);
            textLabel.SetText(sourceText);

            //Reset scroll
            textLabel.maxVisibleCharacters = 0;
            currentCharacter = 0f;
            textLength = sourceText.Length;
            Debug.Log("new textlength is " + textLength);
        }

        /// <summary>
        /// Sets the speaker name.
        /// </summary>
        /// <param name="sourceText">The name to be displayed</param>
        public void SetName(string sourceText)
        {
            Debug.Log("setname");
            if (nameLabel.text != sourceText && nameLabel.text != "")
            {
                //Jostle the textbox when the speaker changes
                animator.SetTrigger("jostle");
            }

            //Apply text to label
            nameLabel.SetText(sourceText);
        }



        /// <summary>
        /// Checks if the per character scrolling reached the end of the text.
        /// </summary>
        /// <returns></returns>
        private bool isEndOfText()
        {
            Debug.Log("isendoftext");
            return (currentCharacter >= textLength);
        }

        /// <summary>
        /// Called when the line advances. Invokes the onAdvance event.
        /// </summary>
        public void AdvanceLine()
        {
            Debug.Log("advanceline");
            advanceArrow.SetVisible(false);
            onAdvance.Invoke();
        }
    }
}