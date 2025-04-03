using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoalBehaviour : MonoBehaviour
{
    public bool isHeavy = false;
    [SerializeField] private UnityEvent _onGoalReached = new UnityEvent();

    public void onGoalReached()
    {
        Debug.Log("Invoking");
        _onGoalReached.Invoke();
    }
}
