using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LeverBehaviour : MonoBehaviour
{

    // public variables
    public float switchRadius = 0.1f;

    // the player
    private Rigidbody2D player;
    private PlayerBehaviour playerScript;
    [SerializeField]private GateBehaviour gate;

    [SerializeField]private KeyCode _openGate = KeyCode.Z;

    [SerializeField]UnityEvent leverPulled = new UnityEvent();

    [SerializeField] [HideInInspector] private bool isLeverPulled = false;
    public bool IsLeverPulled { get { return isLeverPulled; } }



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
    }

    // Update is called once per frame
    void Update()
    {
        player = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        playerScript = PlayerManager.Instance.CurrentCharacter.GetComponent<PlayerBehaviour>();
        if (player != null && Mathf.Abs(Vector2.Distance(player.position, transform.position)) <= switchRadius && Input.GetKeyDown(_openGate))
        {
            LeverPulled();
            CameraManager.Instance.ShakeCamera(0.1f, 0.1f);
        }
    }
    void LeverPulled()
    {
        // todo: play sound / animation?

        // gate open
        // destroy switch
        // play lever pulled sound

        // log that the lever has been pulled
        PlayerPrefs.SetInt(gameObject.scene.name + gameObject.name, 1);



        //Envoke the Unity Events
        leverPulled.Invoke();
        isLeverPulled = true;
        
        //playerScript.collectKey();
        Destroy(gameObject);

    }
}
