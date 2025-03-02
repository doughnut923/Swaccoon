using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    //singleton instance
    public static CameraManager Instance { get; private set; }

    [SerializeField] private Transform Camera;

    [SerializeField] private List<Transform> CameraPositions;
    [SerializeField] private float CameraSpeed = 0.1f;
    [SerializeField] private float MaxX = 100f;
    [SerializeField] private float MinX = -100f;
    private GameObject player;

    void Awake(){
        //set the singleton instance
        Instance = this;
    }

    void Update(){
        // //update the player to the current player according to the player manager
        // player = PlayerManager.Instance.CurrentCharacter;
        
        // //for each position of the camera, calculate the distance between the player and the camera position, set the camera position to the closest one to the player
        // Vector3 closestCamera = CameraPositions[0].position;
        // foreach (Transform position in CameraPositions){
        //     if (Vector3.Distance(player.transform.position, position.position) < Vector3.Distance(player.transform.position, closestCamera)){
        //         closestCamera = position.position;
        //     }
        // }
        // Camera.position = Vector3.Lerp(Camera.position, closestCamera, 0.1f);

        //Move ease the Camera Horizontally to the mean of the character's position
        float meanPosition = 0;
        foreach (GameObject character in PlayerManager.Instance.SwappableCharacters){
            meanPosition += character.transform.position.x;
        }
        meanPosition /= PlayerManager.Instance.SwappableCharacters.Count;
        if(meanPosition > MaxX){
            meanPosition = MaxX;
        }
        if(meanPosition < MinX){
            meanPosition = MinX;
        }

        //scale the size of the camera depending on the distance between the characters
        float maxDistance = 0;
        foreach (GameObject character in PlayerManager.Instance.SwappableCharacters){
            foreach (GameObject otherCharacter in PlayerManager.Instance.SwappableCharacters){
                if (Vector3.Distance(character.transform.position, otherCharacter.transform.position) > maxDistance){
                    maxDistance = Vector3.Distance(character.transform.position, otherCharacter.transform.position);
                }
            }
        }

        //Scale the camera on a scale of 7-10 depending on the distance between the characters
        Camera.GetComponent<Camera>().orthographicSize = Mathf.Lerp(Camera.GetComponent<Camera>().orthographicSize, 7 + (maxDistance / 100), CameraSpeed);

        //ease into the mean position
        Camera.position = Vector3.Lerp(Camera.position, new Vector3(meanPosition, Camera.position.y, Camera.position.z), 0.1f);
        
    }

    public void ShakeCamera(float duration, float magnitude){
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude){
        Vector3 originalPos = Camera.position;
        float elapsed = 0.0f;
        while (elapsed < duration){
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            Camera.position = new Vector3(Camera.position.x + x, Camera.position.y + y, Camera.position.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.position = originalPos;
    }
}
