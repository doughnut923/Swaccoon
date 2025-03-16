using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCinematic : MonoBehaviour
{
    public Transform objectToShake;
    public float shakeDuration = 1f;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    public float timeElapsed = 0.0f;
    public bool doneShake = false;

    public void Play(){

        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        Vector3 originalPos = objectToShake.localPosition;

        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;

            objectToShake.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        objectToShake.localPosition = originalPos;
    }

    void Update(){
        if (timeElapsed < shakeDuration){
            timeElapsed += Time.deltaTime;
            //shake the object
        }
        else{
            doneShake = true;
        }
    }

    public bool CheckDone(){
        //check if the shake is done
        return doneShake;
    }
}
