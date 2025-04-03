using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class EnterTrigger : MonoBehaviour
{

    public UnityEvent onEnterTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Player"){
            onEnterTrigger.Invoke();
        }
    }
}
