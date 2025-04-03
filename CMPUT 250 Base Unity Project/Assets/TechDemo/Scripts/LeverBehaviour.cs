using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum CurrentLeverState
{
    UP,
    DOWN
}


public class LeverBehaviour : MonoBehaviour
{
    public CurrentLeverState _leverState;

    private static LeverBehaviour _instance;
    public static LeverBehaviour Instance
    {
        get
        {
            return _instance;
        }
    }

    protected SokobanBehaviour sokobanScript;

    // public variables
    public float switchRadius = 0.1f;

    // the player
    private Rigidbody2D player;
    private PlayerBehaviour playerScript;
    [SerializeField]private GateBehaviour gate;
    public static CurrentLeverState leverState { get; set; } = CurrentLeverState.UP;

    [SerializeField]private KeyCode _openGate = KeyCode.Z;

    [SerializeField]UnityEvent leverPulled = new UnityEvent();

    [SerializeField] [HideInInspector] private bool isLeverPulled = false;
    public bool IsLeverPulled { get { return isLeverPulled; } }

    [SerializeField] public float leverTimeLimit = 10f;
    private float timeRemaining;

    public SpriteRenderer spriteRenderer;
    protected SpriteRenderer currentSprite;
    [Header("Lever Settings")]
    [SerializeField] protected List<Sprite> leverSprite = new List<Sprite>(2);

    private GateBehaviour closeGate;

    [SerializeField] private AudioSource leverSoundSource;
    [SerializeField] private AudioClip leverPulledSound;

    private bool gateOpening = false;
    [SerializeField]private bool _enabled = true;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        playerScript = PlayerManager.Instance.CurrentCharacter.GetComponent<PlayerBehaviour>();
        //player = (Rigidbody2D)GameObject.Find("Player").GetComponent("Rigidbody2D");
        //playerScript = (PlayerBehaviour)player.gameObject.GetComponent(typeof(PlayerBehaviour));
        //gate = gameObject.GetComponent<GateBehaviour>();
        if(gate == null){
            Debug.LogError("Target gate is not set.");
        }
        _leverState = CurrentLeverState.UP;
        // spriteRenderer = GetComponent<SpriteRenderer>();
        currentSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        sokobanScript = (SokobanBehaviour)player.gameObject.GetComponent(typeof(SokobanBehaviour));

        timeRemaining = leverTimeLimit;

        //closeGate = GateBehaviour.Instance.GetComponent<Rigidbody2D>();
        //spriteRenderer.sprite = leverSprite[0];
    }

    // Update is called once per frame
    void Update()
    {
        //Dim the Lever and make the player unable to interact with it if the lever is disabled
        if(!_enabled){

            currentSprite.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

            return;
        }

        player = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        playerScript = PlayerManager.Instance.CurrentCharacter.GetComponent<PlayerBehaviour>();
        if (player != null && Mathf.Abs(Vector2.Distance(player.position, transform.position)) <= switchRadius && Input.GetKeyDown(_openGate))
        {
            if (gameObject.name == "PlatformLever" && sokobanScript.puzzleComplete==true && _leverState != CurrentLeverState.DOWN)
            {
                _leverState = CurrentLeverState.DOWN;
                LeverPulled();
                CameraManager.Instance.ShakeCamera(0.1f, 0.1f);

            }
            // player can't pull if it is already down
            else if (gameObject.name != "PlatformLever" && _leverState != CurrentLeverState.DOWN)
            {
                _leverState = CurrentLeverState.DOWN;
                LeverPulled();
                CameraManager.Instance.ShakeCamera(0.1f, 0.1f);
                
            }
        }
    }

    // Enable the Lever 
    public void EnableLever(){
        currentSprite.color = new Color(1f, 1f, 1f, 1f);
        _enabled = true;
    }

    void LeverPulled()
    {
        // todo: play sound / animation?

        // gate open
        // destroy switch
        // play lever pulled sound
        Debug.Log("lever was pulled");

        // log that the lever has been pulled
    
        PlayerPrefs.SetInt(gameObject.scene.name + gameObject.name, 1);

        leverSoundSource.clip = leverPulledSound;
        leverSoundSource.volume = 1f;
        leverSoundSource.Play();
        leverSoundSource.Stop();
        //Envoke the Unity Events
        leverPulled.Invoke();
        isLeverPulled = true;

        gateOpening = true;
        Debug.Log("gate opening is " + gateOpening);
        if (_leverState == CurrentLeverState.DOWN)
        {
            // if lever is down (open) then timer begins. will switch lever to up once timer is over
            //Debug.Log("in leverpulled if statement");
            currentSprite.sprite = leverSprite[1];
            //sokobanScript.Win();
            LeverDownTimer();
        }

        //playerScript.collectKey();
        //Destroy(gameObject);

    }
    private void LeverDownTimer()
    {
        // begin timer for how long to keep the gate open
        //Debug.Log("function lever down timer");
        //Debug.Log(leverTimeLimit);
        if (timeRemaining > 0)
        {
            //Debug.Log("lever pulled timer is at "+timeRemaining);

            timeRemaining--;
            Invoke("LeverDownTimer", 1f);
        }
        else
        {
            _leverState = CurrentLeverState.UP;
            //gateOpening = false;
            //Debug.Log(_leverState);
            currentSprite.sprite = leverSprite[0];
            gate.Lock();
            isLeverPulled = false;
            timeRemaining = leverTimeLimit;
        }
    }
}
