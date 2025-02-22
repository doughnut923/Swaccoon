using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletionTrigger : MonoBehaviour
{
    public SwacoonNarrative.SwacoonDialogueTrigger dialogueTrigger;
    private SokobanBehaviour sokobanScript;
    private Rigidbody2D player;
    //private LeverBehaviour leverPulled;
    //private static TriggerHandler _instance;
    //public static TriggerHandler Instance
    //{
    //    get
    //    {
    //        return _instance;
    //    }
    //}

    //[SerializeField] [HideInInspector] public bool isOpen = false;

    //void Awake()
    //{
    //    if (_instance == null)
    //    {
    //        _instance = this;
    //    }


    //}
    // Start is called before the first frame update
    void Start()
    {
        //sokobanScript = (SokobanBehaviour)player.gameObject.GetComponent(typeof(SokobanBehaviour));
        //Debug.Log("sokoban script is " + sokobanScript);

        //player = (Rigidbody2D)GameObject.Find("Player").GetComponent("Rigidbody2D");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //when player enters the trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check winning condition
        // are all the boxes on the goals?
        // if yes and in the area --> win game
        // if no and in the area --> nothing happens

        // check if the player (racoon) is triggering the zone, since only the racoon can win
        if (other.name == "Player")
        {
            sokobanScript = (SokobanBehaviour)other.gameObject.GetComponent(typeof(SokobanBehaviour));
            Debug.Log("sokoban scritp is " + sokobanScript);
            Debug.Log("was the puzzle complete " + sokobanScript.puzzleComplete);
            if (sokobanScript.puzzleComplete == true)
                //now checks if all the boxes are on the goals
            {
                //dialogueTrigger.Trigger();

                // show game over UI
                GameOverUIBehavior.instance.ShowGameOverUI();
                dialogueTrigger.Trigger(); // triggers level over cutscene
            }
        }
    }
}

