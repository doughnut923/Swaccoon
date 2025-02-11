using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    PLAYING,
    PAUSED,
    GAME_OVER,
    LEVEL_COMPLETE
}

public class GameStateManager : MonoBehaviour
{

    public static GameStateManager instance{get; private set;}

    public GameState gameState = GameState.PLAYING;

    // Update is called once per frame

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    void Update()
    {
        if(gameState == GameState.GAME_OVER)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

        }
        
    }
}
