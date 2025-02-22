using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanBehaviour : MonoBehaviour
{
    // Internal Parameters
    private int _boxes;
    private int _goals;

    // Check if Sokoban puzzle complete
    public bool puzzleComplete;

    // Sound for puzzle complete
    [SerializeField] private AudioSource puzzleSoundSource;
    [SerializeField] private AudioClip boxCompleteClip;
    [SerializeField] private AudioClip puzzleCompleteClip;

    public SwacoonNarrative.SwacoonDialogueTrigger endLocationDialogue;

    public LeverBehaviour lever;

    // Start is called before the first frame update
    void Start()
    {
        puzzleComplete = false;

        VerifyCrates();
    }

    void VerifyCrates()
    {
        _goals = GameObject.FindGameObjectsWithTag("Goal").Length;
        _boxes = GameObject.FindGameObjectsWithTag("Crate").Length;

        if (_boxes != _goals)
        {
            Debug.LogError(("Number of boxes and pressure plates for the puzzle are not equal. \nNumber of boxes: " + _boxes + " Number of Goals: " + _goals));
        }
    }

    public void DecrementGoals()
    {
        _goals--;

        //if (_goals <= 0 && lever.IsLeverPulled)
        //{
        //    Debug.Log("Puzzle complete!");
        //    puzzleComplete = true;

        //    puzzleSoundSource.clip = puzzleCompleteClip;
        //    puzzleSoundSource.Play();
            

        //    // show game over UI
        //    GameOverUIBehavior.instance.ShowGameOverUI();
        //    endLocationDialogue.Trigger();
        //}
        //else
        //{
        //    puzzleSoundSource.clip = boxCompleteClip;
        //    puzzleSoundSource.Play();
        //}
        puzzleSoundSource.clip = boxCompleteClip;
        puzzleSoundSource.Play();
    }

    public void Win()
    {
        if (_goals <= 0)
        {
            Debug.Log("Puzzle complete!");
            puzzleComplete = true;

            puzzleSoundSource.clip = puzzleCompleteClip;
            puzzleSoundSource.Play();
        }
    }

    public void DecrementCrates()
    {
        _boxes--;
    }
}