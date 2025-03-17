using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HorseTrigger : MonoBehaviour
{
    public UnityEvent onHorseTriggerEnter;
    
    public void OnTriggerEnter2D(Collider2D c){
        Debug.Log("Horse Triggered");
        if(c.gameObject.name == "Horse"){
            onHorseTriggerEnter?.Invoke();
        }
    }

}
