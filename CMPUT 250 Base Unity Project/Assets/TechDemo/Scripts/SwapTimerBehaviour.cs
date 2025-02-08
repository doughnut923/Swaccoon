using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwapTimerBehaviour : MonoBehaviour
{
    [SerializeField] private Text Timer;

    void Update(){
        Timer.text = ((int)PlayerManager.Instance.swapTimer).ToString();
    }
}
