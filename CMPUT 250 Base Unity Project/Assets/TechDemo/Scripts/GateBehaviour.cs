using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateBehaviour : MonoBehaviour
{
    // Types of Door
    public enum Condition {GateOpen, Key, Puzzle};
    public Condition openCondition;

    [Header("Key Options")]
    public float openRadius = 1f;

    // internal variables
    //private int _remainingEnemies = 0;
    public int _remainingLevers = 0;

    // the player
    private Rigidbody2D player;
    private PlayerBehaviour playerScript;
    private SokobanBehaviour sokobanScript;

    // Start is called before the first frame update
    void Start()
    {
        // if we've already been opened in a past life, don't open again!
        if (PlayerPrefs.HasKey(gameObject.scene.name + gameObject.name) && SceneManager.sceneCount != 1){
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

        
    }

    // Update is called once per frame
    void Update()
    {
        if (openCondition == Condition.GateOpen)
        {
            if (_remainingLevers <= 0){
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
        if (openCondition == Condition.Key){
            playerScript.unlockGate(true);
        }
        else{
            playerScript.unlockGate(false);
        }

        // log that this door has been opened
        PlayerPrefs.SetInt(gameObject.scene.name + gameObject.name, 1);

        Destroy(gameObject);
    }
}
