using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public SwacoonNarrative.SwacoonDialogueTrigger dialogueTrigger;
    public LeverBehaviour lever;
    public GateBehaviour gate;
    private SokobanBehaviour sokobanScript;
    //private Rigidbody2D player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //when player enters the trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        sokobanScript = (SokobanBehaviour)other.gameObject.GetComponent(typeof(SokobanBehaviour));
        //Debug.Log("entering trigger zone");
        //Debug.Log("player " + other.name);
        //Debug.Log("puzzle complete yet " + sokobanScript.puzzleComplete);
        if (other.CompareTag("Player"))
        {
            //Debug.Log("game object name is " + gameObject.name);
            if (gameObject.tag == "Lever Trigger")
            {
                //Debug.Log("compare tag");
                if (!lever.IsLeverPulled)
                {
                    dialogueTrigger.Trigger();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            if (gameObject.tag == "Gate Trigger")
            {
                if (!lever.IsLeverPulled)
                {
                    dialogueTrigger.Trigger();

                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        if (other.name != "Fish")
        {
            //Debug.Log("player name is " + other.name);
            Debug.Log("game obejcte tag is " + gameObject.tag);
            if (gameObject.tag == "Boat Dialogue")
            {
                if (other.CompareTag("Player"))
                {
                    Debug.Log("triggering boat dialogue");
                    dialogueTrigger.Trigger();

                    Destroy(this);
                }
            }
            if (gameObject.tag == "Platform Dialogue")
            { //only show dialogue after the boxes are in place
                //Debug.Log("is the puzzle complete? " + sokobanScript.puzzleComplete);
                if (sokobanScript.puzzleComplete)
                {
                    Debug.Log("triggering platform dialogue");
                    dialogueTrigger.Trigger();
                    Destroy(this);
                }
            }
        }
    }    

}

