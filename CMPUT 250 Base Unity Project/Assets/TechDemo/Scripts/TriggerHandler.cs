using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public SwacoonNarrative.SwacoonDialogueTrigger dialogueTrigger;
    public LeverBehaviour lever;
    public GateBehaviour gate;
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    //when player enters the trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("entering trigger zone");
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
                    Destroy(this);
                }
            }
        }
    }
}

