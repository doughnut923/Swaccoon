using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform Camera;

    [SerializeField] private List<Transform> CameraPositions;
    private GameObject player;

    void Update(){
        //update the player to the current player according to the player manager
        player = PlayerManager.Instance.CurrentCharacter;
        
        //for each position of the camera, calculate the distance between the player and the camera position, set the camera position to the closest one to the player
        Vector3 closestCamera = CameraPositions[0].position;
        foreach (Transform position in CameraPositions){
            if (Vector3.Distance(player.transform.position, position.position) < Vector3.Distance(player.transform.position, closestCamera)){
                closestCamera = position.position;
            }
        }
        Camera.position = Vector3.Lerp(Camera.position, closestCamera, 0.1f);
        
    }
}
