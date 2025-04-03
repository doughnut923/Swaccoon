using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartHorseLeve : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       PlayerManager.Instance.SwappableCharacters[1].GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.SWAPPED_OUT; 
    }

}
