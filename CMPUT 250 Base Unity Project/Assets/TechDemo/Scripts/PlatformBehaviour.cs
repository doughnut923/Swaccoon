using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{

    private Vector3 TargetPosition;
    private Vector3 StartPosition;
    
    [SerializeField] AnimationCurve riseCurve;
    [SerializeField] AnimationCurve fallCurve;
    [SerializeField] float riseTime = 1f;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] public float platformTimeLimit = 10f;
    private float timeRemaining;

    [SerializeField] private AudioSource platformSoundSource;
    [SerializeField] private AudioClip platformRiseSound;
    private float minPitch = 0.5f;
    private float maxPitch = 1f;

    void Awake(){
        TargetPosition = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y-10, transform.position.z);
        StartPosition = transform.position;
        spriteRenderer.color = new Color(1, 1, 1, 0);
        // set timeRemaining
        timeRemaining = platformTimeLimit;
    }
    public void Rise()
    {
        //Invoke("FallTimer", 1f);
        FallTimer();
        CameraManager.Instance.ShakeCamera(riseTime, .1f);
        StartCoroutine(RisePlatform());
    }

    IEnumerator RisePlatform()
    {
        //use the curve to move the platform up also change the opcaity of the platform
        
        float time = 0;
        while (time < riseTime)
        {
            platformSoundSource.clip = platformRiseSound;
            platformSoundSource.volume = 0.5f;

            time += Time.deltaTime;
            float t = time / riseTime;

            platformSoundSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
            platformSoundSource.Play();
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, riseCurve.Evaluate(t));
            spriteRenderer.color = new Color(1, 1, 1, riseCurve.Evaluate(t));
            yield return null;
        }
        // call function to handle when to lower the platform
        //Debug.Log("starting fall timer");
        //Invoke("FallTimer", 1f);
    }
    public void FallTimer()
    {
        // begin timer for how long to keep the platform raised
        // https://discussions.unity.com/t/simple-timer/56201/3
        if (timeRemaining > 0)
        {
            //Debug.Log("platform timer is "+timeRemaining);
            timeRemaining--;
            Invoke("FallTimer", 1f);

        }
        else
        {
            StartPlatformFall();
            // and reset timer
            timeRemaining = platformTimeLimit;
        }
    }
    public void StartPlatformFall()
    {
        //begin lowering the platform
        CameraManager.Instance.ShakeCamera(riseTime, .1f);
        StartCoroutine(FallPlatform());
    }

    IEnumerator FallPlatform()
    {
        //use the curve to move the platform down, also change the opcaity of the platform to invisible
        float time = 0;
        while (time < riseTime)
        {
            platformSoundSource.clip = platformRiseSound;
            platformSoundSource.volume = 0.5f;

            time += Time.deltaTime;
            float t = time / riseTime;

            platformSoundSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
            platformSoundSource.Play();
            Debug.Log("fall curve is " + fallCurve.Evaluate(t));
            transform.position = Vector3.Lerp(TargetPosition, StartPosition, fallCurve.Evaluate(t));
            spriteRenderer.color = new Color(1, 1, 1, fallCurve.Evaluate(t));
           
            yield return null;
        }
    }
}
