using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class GameOverUIBehavior : MonoBehaviour
{

    //create instance
    public static GameOverUIBehavior instance { get; private set; }

    [SerializeField] private Image BackgroundImage; 
    [SerializeField] private Text GameOverText;
    [SerializeField] private Text PlayAgainText;
    [SerializeField] private AnimationCurve fadeInCurve;

    void Awake()
    {
        instance = this;
    }

    public void ShowGameOverUI()
    {
        //show the UI elements
        GameStateManager.instance.gameState = GameState.GAME_OVER;

        Debug.Log("game over UI");

        BackgroundImage.gameObject.SetActive(true);
        GameOverText.gameObject.SetActive(true);
        PlayAgainText.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        Debug.Log("fade in begins");
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime;
            //change alpha of the UI elements
            BackgroundImage.color = new Color(0, 0, 0, fadeInCurve.Evaluate(time) * .7f);
            GameOverText.color = new Color(1, 1, 1, fadeInCurve.Evaluate(time));
            PlayAgainText.color = new Color(1, 1, 1, fadeInCurve.Evaluate(time));
            yield return null;
        }
    }

}