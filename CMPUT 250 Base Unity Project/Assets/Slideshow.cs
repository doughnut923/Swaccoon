using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Slideshow : MonoBehaviour
{
    
    public UnityEvent onPlay;
    public UnityEvent onDone;

    private bool doneFlag = false;

    public float slideduration = 3.0f;
    public float slideFadeSpeed = 0.5f;
    public Image image;
    

    void Start()
    {
        image.canvasRenderer.SetAlpha(0.0f);
    }

    public void Play()
    {
        onPlay?.Invoke();
        StartCoroutine(PlayCoroutine());
    }

    public IEnumerator PlayCoroutine()
    {
        //initialize alpha
        Debug.Log("Playing slideshow");
        image.CrossFadeAlpha(1, slideFadeSpeed, true);
        yield return new WaitForSeconds(slideduration);
        image.CrossFadeAlpha(0, slideFadeSpeed, true);
        doneFlag = true;
    }

    public bool checkDone()
    {
        if (doneFlag){
            onDone?.Invoke();
        }

        return doneFlag;
        
    }
}
