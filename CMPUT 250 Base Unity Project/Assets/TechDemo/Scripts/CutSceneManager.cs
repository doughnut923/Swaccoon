using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Narrative;
using JetBrains.Annotations;
using System;

/// <summary>
/// An inline struct to contain cinematic steps
/// </summary>
/// 
public enum CutsceneState
{
    INTERACT,
    DIALOGUE,
    CINEMATIC,
    SLIDESHOW,
    SHAKE_OBJECT,
}

[System.Serializable]
public struct CinematicStep
{
    public Vector3 location;
    //public string dialogue;
    //public float timeAtLocation;
}

[System.Serializable]
public struct InteractStep
{
    // inline stuct to contain the interaction steps player can take while in the cutscene
    // like punching
    // swapping
    // movement
    // pulling a lever

}

public class CutSceneManager : MonoBehaviour
{

    public static CutSceneManager instance { get; private set; }

    [SerializeField] public List<CutSceneItem> cutSceneItems;
    private CutSceneItem currentCutSceneItem;

    public bool cinematicControlled = true;

    private int _cutsceneIndex = 0;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 1.0f;

    [SerializeField] private string nextScene;
    private bool exit = true;
    public bool canMove = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        
        FadeInScene();
        //Initial Play of the CutScene
        currentCutSceneItem = cutSceneItems[_cutsceneIndex];
        currentCutSceneItem.Play();
    }

    void FadeInScene()
    {
        //Fade in the screen
        fadeImage.canvasRenderer.SetAlpha(1.0f);
        fadeImage.CrossFadeAlpha(0, fadeSpeed, false);
    }

    private IEnumerator FadeOutAndNextScene()
    {
        //Fade in the screen
        fadeImage.canvasRenderer.SetAlpha(0.0f);
        fadeImage.CrossFadeAlpha(1, fadeSpeed, false);
        yield return new WaitForSeconds(fadeSpeed + 3f);
        SceneManager.LoadScene(nextScene);
    }

    


    public void FixedUpdate()
    {
        if (currentCutSceneItem.CheckDone())
        {                            //Check if the current cutscene item is done

            
            
            _cutsceneIndex += 1;                                        //Move to the next cutscene item
            if (_cutsceneIndex < cutSceneItems.Count)
            {
                //Destroy last cutscene item
                // Destroy(cutSceneItems[_cutsceneIndex - 1].CutSceneObject);
                currentCutSceneItem = cutSceneItems[_cutsceneIndex];
                currentCutSceneItem.Play();                             //Note that the play fucntion should only be called once
            }
            else
            {
                //Fade out and load next scene
                if(exit){
                    exit = false;
                    StartCoroutine(FadeOutAndNextScene());
                }
            }
        }
    }
}


[Serializable]
public class CutSceneItem
{
    //The item could either be a cinematic, a dialogue, or an interactable

    public GameObject CutSceneObject;
    public CutsceneState cutsceneState;

    // public void Play();
    // public bool CheckDone();



    public void Play()
    {
        if (cutsceneState == CutsceneState.CINEMATIC)
        {
            Debug.Log("Playing Cinematic:" + CutSceneObject.name);
            CutSceneObject.GetComponent<Cinematic>().Play();
            CutSceneManager.instance.canMove = false;
        }
        else if (cutsceneState == CutsceneState.DIALOGUE)
        {
            Debug.Log("Playing Dialogue:" + CutSceneObject.name);
            CutSceneObject.GetComponent<Dialogue>().Play();
            CutSceneManager.instance.canMove = false;
        }
        else if (cutsceneState == CutsceneState.INTERACT)
        {
            Debug.Log("Playing Interaction:" + CutSceneObject.name);
            CutSceneObject.GetComponent<InteractionScene>().Play();
            CutSceneManager.instance.canMove = true;
        }
        else if (cutsceneState == CutsceneState.SHAKE_OBJECT)
        {
            Debug.Log("Playing Shaking Object:" + CutSceneObject.name);
            CutSceneObject.GetComponent<ShakeCinematic>().Play();
            CutSceneManager.instance.canMove = false;
        }
        else if (cutsceneState == CutsceneState.SLIDESHOW)
        {
            Debug.Log("Playing Slideshow:" + CutSceneObject.name);
            CutSceneObject.GetComponent<Slideshow>().Play();
            CutSceneManager.instance.canMove = false;
        }
        else
        {
            Debug.LogError("Please provide a valid cutscene state");
        }
    }

    public IEnumerator PlayCourotine()
    {
        if (cutsceneState == CutsceneState.CINEMATIC)
        {
            CutSceneObject.GetComponent<Cinematic>().Play();
        }
        else if (cutsceneState == CutsceneState.DIALOGUE)
        {
            CutSceneObject.GetComponent<Dialogue>().Play();
        }
        else if (cutsceneState == CutsceneState.INTERACT)
        {
            CutSceneObject.GetComponent<InteractionScene>().Play();
        }
        else if (cutsceneState == CutsceneState.SHAKE_OBJECT)
        {
            CutSceneObject.GetComponent<ShakeCinematic>().Play();
        }
        else if (cutsceneState == CutsceneState.SLIDESHOW)
        {
            CutSceneObject.GetComponent<Slideshow>().Play();
        }
        else
        {
            Debug.LogError("Please provide a valid cutscene state");
        }
        yield return null;
    }

    public bool CheckDone()
    {
        if (cutsceneState == CutsceneState.CINEMATIC)
        {
            return CutSceneObject.GetComponent<Cinematic>().CheckDone();
        }
        else if (cutsceneState == CutsceneState.DIALOGUE)
        {
            return CutSceneObject.GetComponent<Dialogue>().CheckDone();
        }
        else if (cutsceneState == CutsceneState.INTERACT)
        {
            return CutSceneObject.GetComponent<InteractionScene>().CheckDone();
        }
        else if (cutsceneState == CutsceneState.SHAKE_OBJECT)
        {
            return CutSceneObject.GetComponent<ShakeCinematic>().CheckDone();
        }
        else if (cutsceneState == CutsceneState.SLIDESHOW)
        {
            return CutSceneObject.GetComponent<Slideshow>().checkDone();
        }
        else
        {
            Debug.LogError("Please provide a valid cutscene state");
            return false;
        }
    }

}
