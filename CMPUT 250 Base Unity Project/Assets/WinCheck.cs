using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCheck : MonoBehaviour
{
    //public SwacoonNarrative.SwacoonDialogueTrigger dialogueTrigger;

    [SerializeField] private UnityEngine.UI.Image fadeImage;
    [SerializeField] private float fadeSpeed = 1.5f;
    [SerializeField] private string nextScene;

    bool faded = false;

    private IEnumerator FadeOutAndNextScene()
    {
        //Fade in the screen
        fadeImage.canvasRenderer.SetAlpha(0.0f);
        fadeImage.CrossFadeAlpha(1, fadeSpeed, false);
        yield return new WaitForSeconds(fadeSpeed + 3f);
        SceneManager.LoadScene(nextScene);
    }

    

    //check if the Diague is done, if so, fade out the screen
    void Update()
    {
        //if (dialogueTrigger.isDialogueDone && !faded)
        if (!faded)
        {
            faded = true;
            StartCoroutine(FadeOutAndNextScene());
        }
    }
}
