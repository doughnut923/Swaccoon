using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeverBehaviour : MonoBehaviour
{

    // public variables
    public float switchRadius = 0.1f;

    // the player
    private Rigidbody2D player;
    private PlayerBehaviour playerScript;
    private GateBehaviour gate;

    private KeyCode _openGate = KeyCode.Z;

    // Start is called before the first frame update
    void Start()
    {
        player = (Rigidbody2D)GameObject.Find("Player").GetComponent("Rigidbody2D");
        playerScript = (PlayerBehaviour)player.gameObject.GetComponent(typeof(PlayerBehaviour));
        //gate = gameObject.GetComponent<GateBehaviour>();
        gate = FindObjectOfType<GateBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (player != null && Mathf.Abs(Vector2.Distance(player.position, transform.position)) <= switchRadius && Input.GetKeyDown(_openGate))
        {
            LeverPulled();
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
        
        //Debug.Log("gate is type " + gate);

        gate.leverPulled();

        Destroy(gameObject);
        
        //playerScript.collectKey();

    }
}
