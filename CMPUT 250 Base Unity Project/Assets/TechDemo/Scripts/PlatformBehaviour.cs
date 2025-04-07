using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{

    private Vector3 TargetPosition;
    private Vector3 StartPosition;

    [SerializeField] AnimationCurve riseCurve;
    [SerializeField] float riseTime = 1f;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] public float platformTimeLimit = 10f;
    private float timeRemaining;

    [SerializeField] private AudioSource platformSoundSource;
    [SerializeField] private AudioClip platformRiseSound;
    private float minPitch = 0.5f;
    private float maxPitch = 1f;

    private Rigidbody2D player;
    private PlayerBehaviour playerScript;
    private SokobanBehaviour sokobanScript;
    public bool hasRose = false;
    public float shakeIntensity = 0.01f;

    public PressurePlateBehviour pressurePlate;

    void Awake()
    {
        TargetPosition = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
        StartPosition = transform.position;
        spriteRenderer.color = new Color(1, 1, 1, 0);

        player = (Rigidbody2D)GameObject.Find("Player").GetComponent("Rigidbody2D");
        playerScript = (PlayerBehaviour)player.gameObject.GetComponent(typeof(PlayerBehaviour));
        sokobanScript = (SokobanBehaviour)player.gameObject.GetComponent(typeof(SokobanBehaviour));

        // set timeRemaining
        timeRemaining = platformTimeLimit;

        
    }
    public void Rise()
    {
        if (hasRose)
        {
            //reset the timer
            timeRemaining = platformTimeLimit;
            return;
        }

        hasRose = true;

        //Invoke("FallTimer", 1f);
        Debug.Log("are we sure the puzzle is complete " + sokobanScript.puzzleComplete);
        if (sokobanScript.puzzleComplete == true)
        {
            FallTimer();
            CameraManager.Instance.ShakeCamera(riseTime, .05f);
            StartCoroutine(RisePlatform());
        }
    }

    IEnumerator RisePlatform()
    {
        //use the curve to move the platform up also change the opcaity of the platform
        platformSoundSource.clip = platformRiseSound;
        platformSoundSource.Play();

        float time = 0;
        while (time < riseTime)
        {

            time += Time.deltaTime;
            float t = time / riseTime;

            transform.position = Vector3.Lerp(StartPosition, TargetPosition, riseCurve.Evaluate(t));
            spriteRenderer.color = new Color(1, 1, 1, riseCurve.Evaluate(t));
            yield return null;
        }
    }

    public void ShakePlatform()
    {
        //shake the platform
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0.0f;
        while (elapsed < 0.5f)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * shakeIntensity;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeIntensity;
            transform.position = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPos;
    }

    public void FallTimer()
    {
        // begin timer for how long to keep the platform raised
        // https://discussions.unity.com/t/simple-timer/56201/3
        if (timeRemaining > 0)
        {
            //Debug.Log("platform timer is "+timeRemaining);
            timeRemaining--;
            if (timeRemaining < platformTimeLimit / 2 && timeRemaining > 0)
            {
                ShakePlatform();
            }
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
        hasRose = false;
    }

    IEnumerator FallPlatform()
    {
        //use the curve to move the platform down, also change the opcaity of the platform to invisible
        platformSoundSource.clip = platformRiseSound;
        platformSoundSource.Play();
        pressurePlate.currentPressuePlateSprite.sprite = pressurePlate.pressurePlateSprite[0];

        float time = 0;
        while (time < riseTime)
        {

            time += Time.deltaTime;
            float t = time / riseTime;
            // calculate how transparent the platform should be as it falls. going from solid to transparent (1 to 0)
            float transparency = Mathf.Lerp(1f, 0f, riseCurve.Evaluate(t));

            transform.position = Vector3.Lerp(TargetPosition, StartPosition, riseCurve.Evaluate(t));

            spriteRenderer.color = new Color(1, 1, 1, transparency);


            yield return null;
        }
        
    }
}
