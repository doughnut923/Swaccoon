using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CompletionTrigger : MonoBehaviour
{
    public SwacoonNarrative.SwacoonDialogueTrigger dialogueTrigger;
    private SokobanBehaviour sokobanScript;
    private Rigidbody2D player;

    [SerializeField] private AudioSource levelCompleteSoundSource;
    [SerializeField] private AudioClip levelCompletePulledSound;
    [SerializeField] private AudioSource vortexSoundSource;
    [SerializeField] private AudioClip vortexSound;

    [SerializeField] private float leaveTime = 20f;

    [SerializeField] AnimationCurve vortexCurve;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer shadowSpriteRenderer;
    public UnityEvent onLevelComplete;

    private float minPitch = 0.5f;
    private float maxPitch = 1f;

    private Vector3 TargetPosition;
    private Vector3 StartPosition;

    public string character_name = "Player";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //when player enters the trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check winning condition
        // are all the boxes on the goals?
        // if yes and in the area --> win game
        // if no and in the area --> nothing happens

        // check if the player (racoon) is triggering the zone, since only the racoon can win
        Debug.Log(other.name + " is in the trigger zone");
        if (other.name == character_name)
        {
            sokobanScript = (SokobanBehaviour)other.gameObject.GetComponent(typeof(SokobanBehaviour));
            Debug.Log("sokoban scritp is " + sokobanScript);
            Debug.Log("was the puzzle complete " + sokobanScript.puzzleComplete);
            if (sokobanScript.puzzleComplete == true)
                //now checks if all the boxes are on the goals
            {
                TargetPosition = transform.position;
                transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
                StartPosition = transform.position;
                Debug.Log("leave time is set to " + leaveTime);
                float time = 0;
                while (time < leaveTime)
                {
                    Debug.Log("in while loop time is " + time);
                    vortexSoundSource.clip = vortexSound;
                    vortexSoundSource.volume = 0.5f;

                    time += Time.deltaTime;
                    float t = time / leaveTime;

                    vortexSoundSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
                    vortexSoundSource.Play();
                    //transform.position = Vector3.Lerp(StartPosition, TargetPosition, riseCurve.Evaluate(t));
                    transform.position = Vector3.Lerp(StartPosition, TargetPosition, vortexCurve.Evaluate(t));
                    float transparency = Mathf.Lerp(1f, 0f, vortexCurve.Evaluate(t));
                    spriteRenderer.color = new Color(1, 1, 1, transparency);
                    shadowSpriteRenderer.color = new Color(1, 1, 1, transparency);
                    //yield return new WaitForSeconds(leaveTime);
                }
                StartCoroutine(LevelComplete());
                //dialogueTrigger.Trigger();

                // show game over UI
                //GameOverUIBehavior.instance.ShowGameOverUI();
                //levelCompleteSoundSource.clip = levelCompletePulledSound;
                //levelCompleteSoundSource.volume = 0.5f;
                //levelCompleteSoundSource.Play();
                //dialogueTrigger.Trigger(); // triggers level over cutscene
            }
        }
    }
    IEnumerator LevelComplete()
    {
        //use the curve to move the platform up also change the opcaity of the platform

        //TargetPosition = transform.position;
        //transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
        //StartPosition = transform.position;
        //Debug.Log("leave time is set to " + leaveTime);
        //float time = 0;
        //while (time < leaveTime)
        //{
        //    Debug.Log("in while loop time is " + time);
        //    vortexSoundSource.clip = vortexSound;
        //    vortexSoundSource.volume = 0.5f;

        //    time += Time.deltaTime;
        //    float t = time / leaveTime;

        //    vortexSoundSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
        //    vortexSoundSource.Play();
        //    //transform.position = Vector3.Lerp(StartPosition, TargetPosition, riseCurve.Evaluate(t));
        //    transform.position = Vector3.Lerp(StartPosition, TargetPosition, vortexCurve.Evaluate(t));
        //    float transparency = Mathf.Lerp(1f, 0f, vortexCurve.Evaluate(t));
        //    spriteRenderer.color = new Color(1, 1, 1, transparency);
        //    spriteRenderer.color = new Color(1, 1, 1, transparency);
        //    yield return null;
        //}


        // show game over UI
        // GameOverUIBehavior.instance.ShowGameOverUI();
        onLevelComplete?.Invoke();
        levelCompleteSoundSource.clip = levelCompletePulledSound;
        levelCompleteSoundSource.volume = 0.5f;
        levelCompleteSoundSource.Play();
        dialogueTrigger.Trigger(); // triggers level over cutscene
        yield return null;

    }
}

