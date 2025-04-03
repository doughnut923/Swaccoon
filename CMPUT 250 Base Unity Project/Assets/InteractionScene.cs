using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionScene : MonoBehaviour
{

    //Will be changed to true seperately as the player triggers something in the scene
    public List<bool> flags;

    public UnityEvent OnPlay;
    public UnityEvent OnComplete;

    public bool CheckDone()
    {
        foreach (bool flag in flags)
        {
            if (!flag)
            {
                return false;
            }
        }


        OnComplete?.Invoke();
        return true;
    }

    public void Play()
    {

        List<GameObject> players = PlayerManager.Instance.SwappableCharacters;

        foreach (GameObject player in players)
        {
            // player.GetComponent<PlayerBehaviour>().enabled = true;
            if (player == PlayerManager.Instance.CurrentCharacter)
            {
                player.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.IDLE;
            }
            else
            {
                player.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.SWAPPED_OUT;
            }
        }
        
        //Actually does nothing
        OnPlay?.Invoke();
        //set all flags to false
        for (int i = 0; i < flags.Count; i++)
        {
            flags[i] = false;
        }
        return;
    }

    public void SetFlag(int index)
    {
        flags[index] = true;
    }
}
