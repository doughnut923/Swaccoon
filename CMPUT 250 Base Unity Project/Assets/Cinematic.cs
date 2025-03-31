using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cinematic : MonoBehaviour
{

    // List of character movements in the cinematic
    public List<Movement> movements;
    private bool _doneMovement = true;
    private bool _doneAllMovements = false;

    private bool enableWater = false;
    private bool enableWaterFlag = true;

    public UnityEvent OnPlay;
    public UnityEvent CinematicEnd;

    public bool CheckDone()
    {
        if (_doneAllMovements)
        {
            //enable the PlayerManager and all PlayerBehaviour
            PlayerManager.Instance.enabled = true;
            List<GameObject> players = PlayerManager.Instance.SwappableCharacters;

            foreach (GameObject player in players)
            {
                // player.GetComponent<PlayerBehaviour>().enabled = true;
                if(player == PlayerManager.Instance.CurrentCharacter)
                {
                    player.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.IDLE;
                }else{
                    player.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.SWAPPED_OUT;
                }
            }


            enableWater = true;
            enableWaterFlag = true;
            
            CinematicEnd?.Invoke();
            CutSceneManager.instance.cinematicControlled = false;
            return true;
        }
        return false;
    }

    void Update()
    {
        if (enableWater && enableWaterFlag)
        {
            GameObject[] waters = GameObject.FindGameObjectsWithTag("Water");
            if(waters.Length == 0)
            {
                Debug.LogWarning("No water in the scene");
                return;
            }
            foreach (GameObject water in waters)
            {
                water.GetComponent<WaterBehaviour>().enabled = true;
            }

            enableWaterFlag = false;

        }else if (!enableWater && enableWaterFlag)
        {
            GameObject[] waters = GameObject.FindGameObjectsWithTag("Water");
            if(waters.Length == 0)
            {
                Debug.LogWarning("No water in the scene");
                return;
            }
            foreach (GameObject water in waters)
            {
                water.GetComponent<WaterBehaviour>().enabled = false;
            }

            enableWaterFlag = false;
        }
    }

    public void Play()
    {

        if (movements.Count == 0)
        {
            Debug.LogWarning("No movements in cinematic");
            return;
        }
        //Disable the PlayerManager and all PlayerBehaviour

        PlayerManager.Instance.enabled = false;
        List<GameObject> players = PlayerManager.Instance.SwappableCharacters;


        foreach (GameObject player in players)
        {
            // player.GetComponent<PlayerBehaviour>().enabled = false;
            player.GetComponent<PlayerBehaviour>()._playerState= CurrentPlayerState.CUTSCENE_PLAYING;
            if(player.name == "Fish")
            {
                player.GetComponent<FishBehaviour>()._playerState = CurrentPlayerState.CUTSCENE_PLAYING;
            }
        }

        //Disable all WaterBehaviour on Scene
        enableWater = false;
        enableWaterFlag = true;

        OnPlay?.Invoke();

        //Loop through the movements and move the characters
        StartCoroutine(PlayMovements());
    }

    private IEnumerator PlayMovements()
    {
        int index = 0;
        while (index < movements.Count)
        {
            //Check if the last movement is done
            if (_doneMovement)
            {
                if (index > 0)
                    yield return new WaitForSeconds(movements[index-1].delay);

                _doneMovement = false;
                Movement movement = movements[index];
                // Debug.Log("Moving " + movement.character.name + " to " + movement.toPosition);
                StartCoroutine(MoveCharacterCoroutine(movement.character, movement.toPosition, movement.speed, movement.delay));
                index++;
            }
            yield return null;
        }
        while (true){
            if (_doneMovement)
            {
                _doneAllMovements = true;
                break;
            }
            yield return null;
        }
    }

    

    private IEnumerator MoveCharacterCoroutine(Transform character, Vector3 toPosition, float speed, float delayAfter)
    {
        //Move the player toward the position

        while (character.position != toPosition)
        {
            character.position = Vector3.MoveTowards(character.position, toPosition, speed * Time.deltaTime);
            //set movement to allow animation
            character.GetComponent<PlayerBehaviour>().setMovment(character.position - toPosition);
            character.GetComponent<PlayerBehaviour>().setDirection(PlayerBehaviour.vectTorDir(toPosition - character.position));
            yield return null;
        }

        yield return new WaitForSeconds(delayAfter);

        _doneMovement = true;
        
    }

}


[Serializable]
public class Movement
{
    public Transform character;
    public Vector3 toPosition;
    public float speed;
    public float delay;
}
