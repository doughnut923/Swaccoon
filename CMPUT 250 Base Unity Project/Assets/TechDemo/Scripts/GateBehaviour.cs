using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateBehaviour : MonoBehaviour
{
    // Types of Door
    public enum Condition {GateOpen, Key, Puzzle, GateClosed};
    public Condition openCondition;

    [Header("Key Options")]
    public float openRadius = 1f;

    // internal variables
    //private int _remainingEnemies = 0;
    public int _remainingLevers = 0;

    public bool gateOpen = false;

    // the player
    private Rigidbody2D player;
    private PlayerBehaviour playerScript;
    private SokobanBehaviour sokobanScript;

    [SerializeField] private AudioSource gateSoundSource;
    [SerializeField] private AudioClip gateOpenSound;

    [SerializeField] SpriteRenderer spriteRenderer;

    //public LeverBehaviour leverStatus;
    public LeverBehaviour lever;
    //private LeverBehaviour leverBehave;

    Collider2D gateCollider;

    // Start is called before the first frame update
    void Start()
    {
        // if we've already been opened in a past life, don't open again!
        if (PlayerPrefs.HasKey(gameObject.scene.name + gameObject.name) && SceneManager.sceneCount != 1){
            Debug.Log("detroying game object");
            Destroy(gameObject);
        }

        //****
        //  need to change open condition to if lever is pulled
        ///****
        if (openCondition == Condition.GateOpen)
        {
            _remainingLevers = GameObject.FindObjectsOfType(typeof(GateBehaviour)).Length;
        }

        player = (Rigidbody2D)GameObject.Find("Player").GetComponent("Rigidbody2D");
        playerScript = (PlayerBehaviour)player.gameObject.GetComponent(typeof(PlayerBehaviour));

        // Manually getting sokoban script. Might be better to get the player and call sokoban script when needed
        sokobanScript = (SokobanBehaviour)player.gameObject.GetComponent(typeof(SokobanBehaviour));

        gateCollider = gameObject.GetComponent<Collider2D>();

        //leverStatus = LeverBehaviour.Instance.GetComponent<LeverBehaviour>();
        //leverBehave = GetComponent<LeverBehaviour>();
        //Debug.Log(leverStatus);
    }

    // Update is called once per frame
    void Update()
    {
        if (openCondition == Condition.GateOpen)
        {
            //Unlock();
            //if (_remainingLevers <= 0){
            //    Unlock();
            //}
            if (lever.IsLeverPulled == true)
            {
                Unlock();

            }
        }

        else if (openCondition == Condition.Key){
            if (playerScript.getKeys() > 0 && Mathf.Abs(Vector2.Distance(player.position, transform.position)) <= openRadius){
                Unlock();
            }
        }

        else if (openCondition == Condition.Puzzle)
        {
            if (sokobanScript.puzzleComplete)
            {
                Unlock();
            }
        }

    }

    //void enemyDeath(){
    //    if (openCondition == Condition.GateOpen)
    //    {
    //        _remainingEnemies--;
    //    }
    //}

    public void leverPulled()
    {
        _remainingLevers--;
    }

    void Unlock(){
        // if we're opening with a key, decrement key from player
        //if (openCondition == Condition.GateOpen)
        //{
        //    playerScript.unlockGate(true);
        //}
        //Debug.Log("Unlock function");
        //Debug.Log("is sound playing " + gateSoundSource.isPlaying);
        if(gateOpen){
            Debug.Log("Gate is already open");
            return;
        }

        gateOpen = true;
        
        if (openCondition == Condition.GateOpen){
            if (lever.IsLeverPulled == true)
            {
                Debug.Log("here gate is now opening sound " + lever.IsLeverPulled);
                spriteRenderer.color = new Color(1, 1, 1, 0);
                gameObject.GetComponent<Collider2D>().enabled = false;
                gateCollider.isTrigger = true;
                gateSoundSource.clip = gateOpenSound;
                gateSoundSource.volume = 1f;
                gateSoundSource.Play();
                //Debug.Log("unlock function" + gateCollider.isTrigger);

            }
            //Debug.Log("in open condition is sound playing "+ gateSoundSource.isPlaying);
            //Debug.Log("gate is set to " + gateOpenSound);
            ////playerScript.unlockGate(true);
            //if (lever.gateOpening!=true)
            //{
            //    gateSoundSource.clip = gateOpenSound;
            //    gateSoundSource.volume = 0.5f;
            //    gateSoundSource.Play();
            //}
            //lever.gateOpening = false;
            //if (!gateSoundSource.isPlaying)
            //{
            //    Debug.Log("is the sound really playing " + gateSoundSource.isPlaying);
            //    gateSoundSource.clip = gateOpenSound;
            //    gateSoundSource.volume = 1f;
            //    gateSoundSource.mute = false;
            //    if (gateOpenSound == null)
            //    {
            //        Debug.Log("gate open sound not assigned");
            //    }
            //    //gateSoundSource.Play();
            //    Debug.Log("Attempting to play sound...");
            //    gateSoundSource.Play();
            //    Debug.Log("Sound should be playing now.");
            //    Debug.Log("but the sound is playing now right " + gateSoundSource.isPlaying);
            //}
            //else
            //{
            //    Debug.Log("Audio is already playing!");
            //}

            //gateSoundSource.clip = gateOpenSound;
            ////gateSoundSource.volume = 1f;
            //gateSoundSource.Play();

            //playerScript.unlockGate(true);

        }
        else
        {
            playerScript.unlockGate(false);
        }

        // log that this door has been opened
        PlayerPrefs.SetInt(gameObject.scene.name + gameObject.name, 1);

        
        //(gameObject.GetComponent(typeof(Collider2D)) as Collider2D).isTrigger = true;
        if (lever.IsLeverPulled == true)
        {
            Debug.Log("gate is now opening sound " + lever.IsLeverPulled);
            spriteRenderer.color = new Color(1, 1, 1, 0);
            gameObject.GetComponent<Collider2D>().enabled = false;
            gateCollider.isTrigger = true;
            gateSoundSource.clip = gateOpenSound;
            //gateSoundSource.volume = 1f;
            gateSoundSource.Play();
            //Debug.Log("unlock function" + gateCollider.isTrigger);

        }
        
        //Destroy(gameObject);
    }

    // once the timer is done, closes/locks the door
    public void Lock()
    {
        
        if (lever.IsLeverPulled == true)
        {
            
            spriteRenderer.color = new Color(1, 1, 1, 1);
            
            gateCollider.isTrigger = false;
            gameObject.GetComponent<Collider2D>().enabled = true;
            gateOpen = false;
        }
    }
}
