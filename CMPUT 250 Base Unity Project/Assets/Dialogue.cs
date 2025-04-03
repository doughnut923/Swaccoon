using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{

    public bool flagtocheck = false;

    public SwacoonNarrative.SwacoonDialogueTrigger cutsceneDialogue;

    public UnityEvent OnPlay;
    public UnityEvent OnDialogueEnd;

    public bool CheckDone()
    {
        //Debug.Log("check dialogue");
        // if given dialogue is finished
        // if given dialogue is ongoing --> keeps going
        if (cutsceneDialogue.isDialogueDone == true)
        {
            PlayerManager.Instance.enabled = true;
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
                cutsceneDialogue.isDialogueDone = false;
            }

            OnDialogueEnd?.Invoke();
            return true;
        }
        else
        {
            return false;
        }

    }

    public void Play()
    {
        //Disable the PlayerManager and all PlayerBehaviour

        PlayerManager.Instance.enabled = false;
        List<GameObject> players = PlayerManager.Instance.SwappableCharacters;

        Debug.Log("Disabling Player : " + PlayerManager.Instance.enabled);

        foreach (GameObject player in players)
        {
            // player.GetComponent<PlayerBehaviour>().enabled = false;
            Debug.Log("hello");
            player.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.CUTSCENE_PLAYING;
        }

        OnPlay?.Invoke();
        cutsceneDialogue.Trigger();
        return;
    }
}
