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

    [SerializeField] List<PlayerBehaviour> players = new List<PlayerBehaviour>();

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

            //Handle Undo
            if(Input.GetKeyDown(KeyCode.U))
            {
                foreach (var player in players)
                {
                    player.UndoMove();
                }   
                GameOverUIBehavior.instance.UnshowGameOverUI();
            }
            else if(Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

        }
        
    }
}
