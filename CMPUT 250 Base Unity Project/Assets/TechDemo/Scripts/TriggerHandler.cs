using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public SwacoonNarrative.SwacoonDialogueTrigger dialogueTrigger;
    

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
        if (other.CompareTag("Player"))
        {
            
            Debug.Log("compared was true, what is happening in dialogue trigger ");

            dialogueTrigger.Trigger();
        }
    }
}

