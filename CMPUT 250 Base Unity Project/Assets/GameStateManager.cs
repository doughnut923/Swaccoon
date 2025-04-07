using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    PLAYING,
    PAUSED,
    GAME_OVER,
    LEVEL_COMPLETE,
    RESET_CONFIRM
}

public class GameStateManager : MonoBehaviour
{

    [SerializeField] private UnityEngine.UI.Image fadeImage;
    [SerializeField] private float fadeSpeed = 1.5f;
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

    void Start(){
        if(fadeImage == null)
        {
            Debug.LogWarning("No fade image found, skipping fading");
            return;
        }
        FadeInScene();
    }

    public void FadeInScene()
    {
        //Fade in the screen
        fadeImage.canvasRenderer.SetAlpha(1.0f);
        fadeImage.CrossFadeAlpha(0, fadeSpeed, false);
    }

    public IEnumerator LoadNextScene(String name)
    {
        fadeImage.CrossFadeAlpha(1, fadeSpeed, false);
        yield return new WaitForSeconds(fadeSpeed);
        SceneManager.LoadScene(name);
    }

    public void GoNextScene(String name){
        StartCoroutine(LoadNextScene(name));
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
                gameState = GameState.PLAYING;
            }
            else if(Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

        }
        if(gameState == GameState.PLAYING)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                ResetComfirmUI.instance.ShowResetComfirmUI();
                gameState = GameState.RESET_CONFIRM;
            }
        }

        if(gameState == GameState.RESET_CONFIRM)
        {
            Debug.Log("Checking Key");
            if(Input.GetKeyDown(KeyCode.Y))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if(Input.GetKeyDown(KeyCode.N))
            {
                ResetComfirmUI.instance.UnshowResetComfirmUI();
                PlayerManager.Instance.ReinitalizePlayers();
                gameState = GameState.PLAYING;
            }
        }
        
    }
}
