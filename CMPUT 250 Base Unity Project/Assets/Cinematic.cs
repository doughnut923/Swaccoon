using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematic : MonoBehaviour
{

    // List of character movements in the cinematic
    public List<Movement> movements;
    private bool _doneMovement = true;
    private bool _doneAllMovements = false; 

    public bool CheckDone(){
        if(_doneAllMovements){
            //enable the PlayerManager and all PlayerBehaviour
            PlayerManager.Instance.enabled = true;
            List<GameObject> players = PlayerManager.Instance.SwappableCharacters;

            foreach(GameObject player in players){
                player.GetComponent<PlayerBehaviour>().enabled = true;
            }

            //enable all WaterBehaviour on Scene
            GameObject [] waters = GameObject.FindGameObjectsWithTag("Water");
            foreach (GameObject water in waters)
            {
                water.GetComponent<WaterBehaviour>().enabled = true;
            }

            return true;
        }  
        return false;
    }

    public void Play(){

        Debug.Log("Playing " + name);

        if(movements.Count == 0){
            Debug.LogWarning("No movements in cinematic");
            return;
        }
        //Disable the PlayerManager and all PlayerBehaviour

        PlayerManager.Instance.enabled =false;
        List<GameObject> players = PlayerManager.Instance.SwappableCharacters;

        Debug.Log("Disabling Player : " +  PlayerManager.Instance.enabled);

        foreach(GameObject player in players){
            player.GetComponent<PlayerBehaviour>().enabled = false;
        }

        //Disable all WaterBehaviour on Scene
        GameObject [] waters = GameObject.FindGameObjectsWithTag("Water");
        foreach (GameObject water in waters)
        {
            water.GetComponent<WaterBehaviour>().enabled = false;
        }

        //Loop through the movements and move the characters
        StartCoroutine(PlayMovements());
    }

    private IEnumerator PlayMovements(){
        int index = 0;
        while(index < movements.Count){
            //Check if the last movement is done
            if(_doneMovement){
                _doneMovement = false;
                Movement movement = movements[index];
                StartCoroutine(MoveCharacterCoroutine(movement.character, movement.toPosition, movement.speed, movement.delay));
                index++;
            }
            yield return null;
        }
        _doneAllMovements = true;
    }

    private IEnumerator MoveCharacterCoroutine(Transform character, Vector3 toPosition, float speed, float delayAfter){
        //Move the player toward the position

        while(Vector3.Distance(character.position, toPosition) > 0.1f){
            character.position = Vector3.MoveTowards(character.position, toPosition, speed * Time.deltaTime);
            yield return null;
        }

        character.position = toPosition;
        
        yield return new WaitForSeconds(delayAfter);

        _doneMovement = true;
    }
    
}


[Serializable]
public class Movement{
    public Transform character;
    public Vector3 toPosition;
    public float speed;
    public float delay;
}
