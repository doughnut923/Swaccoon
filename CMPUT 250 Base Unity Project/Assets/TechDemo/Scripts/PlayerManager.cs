using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerManagerState
{
    Swapping,
    NOT_SWAPPING,
    CUTSCENE_PLAYING
}

//The Class basically changes the state of the character based on the input from the player essentially for swapping the character
public class PlayerManager : MonoBehaviour
{

    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public static PlayerManagerState _playerManagerState { get; set; } = PlayerManagerState.NOT_SWAPPING;

    [SerializeField] public GameObject CurrentCharacter { get; private set; }            //Reference to the current character
    [SerializeField] private List<GameObject> SwappableCharacters = new List<GameObject>(3);    //List of characters that can be swapped
    [SerializeField] private List<Image> UIOutlines = new List<Image>(3);             //Reference to list of UI elements displayed for swapping the chaaracter and for the player to see which character they are in control of
    [SerializeField] private int currentIndex = 0;                                              //Index of the current character
    [SerializeField] private int lastCharacter = 0;                                             //Index of the last character
    [SerializeField] private KeyCode startSwap = KeyCode.Q;                                     //Key to start swap the character
    [SerializeField] private KeyCode select = KeyCode.E;                                        //Key for select between character when swapping
    [SerializeField] private KeyCode swap = KeyCode.T;                                   //Key to cancel the swap
    [SerializeField] private KeyCode cancel = KeyCode.Q;                                   //Key to cancel the swap
    [SerializeField] private float swapTimeLimit = 60;

    public float swapTimer;

    public int SwapsLeft = 3;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            CurrentCharacter = SwappableCharacters[currentIndex];
        }
        //swapTimer = swapTimeLimit;
    }

    void Start()
    {

        //// Check for SwappableCharacters being null or empty
        //if (SwappableCharacters == null || SwappableCharacters.Count == 0)
        //{
        //    Debug.LogError("SwappableCharacters is null or empty");
        //    return;
        //}

        //// Check if currentIndex is valid
        //if (currentIndex < 0 || currentIndex >= SwappableCharacters.Count)
        //{
        //    Debug.LogError("currentIndex is out of bounds");
        //    return;
        //}

        //// Check if the current character is null
        //CurrentCharacter = SwappableCharacters[currentIndex];
        //if (CurrentCharacter == null)
        //{
        //    Debug.LogError("Current character at index " + currentIndex + " is null");
        //    return;
        //}

        //Debug.Log("Current character playing is " + CurrentCharacter.name);

        foreach (Image outline in UIOutlines)
        {
            if (outline == UIOutlines[currentIndex])
            {
                outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 1f);
            }
            else
            {
                outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 0.5f);
            }
        }

        foreach (GameObject person in SwappableCharacters)
        {
            if (person != null)
            {
                PlayerBehaviour playerBehaviour = person.GetComponent<PlayerBehaviour>();
                if (playerBehaviour == null)
                {
                    Debug.LogError("PlayerBehaviour component missing on " + person.name);
                    continue; // Skip this character if it doesn't have the PlayerBehaviour component
                }

                if (person != CurrentCharacter)
                {
                    playerBehaviour._playerState = CurrentPlayerState.SWAPPED_OUT;
                }
                else
                {
                    playerBehaviour._playerState = CurrentPlayerState.IDLE;
                }
            }
            else
            {
                Debug.LogError("Character in SwappableCharacters array is null");
            }
        }


        foreach (GameObject character in SwappableCharacters)
        {
            if (character != CurrentCharacter)
            {
                character.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.SWAPPED_OUT;
            }
            else
            {
                character.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.IDLE;
            }
        }
    }

    void Update()
    {
        if (_playerManagerState == PlayerManagerState.CUTSCENE_PLAYING)
        {
            return;
        }
        //if (swapTimer <= 0)
        //{
        //    //Lose
        //    GameOverUIBehavior.instance.ShowGameOverUI();
        //}
        else
        {
            if (!_playerManagerState.Equals(PlayerManagerState.CUTSCENE_PLAYING))
            {
                //swapTimer -= Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(startSwap) && _playerManagerState == PlayerManagerState.NOT_SWAPPING)
        {
            StartSwap();
        }
        else if (Input.GetKeyDown(select))
        {
            SelectCharacter();
        }
        else if (Input.GetKeyDown(swap))
        {
            SwapCharacter();
        }
        else if (Input.GetKeyDown(cancel))
        {
            CancelSwap();
        }
    }

    /**
        * The following function start the swap and outlines the current character and the character that can be swapped to
        */
    private void StartSwap()
    {
        if (SwapsLeft <= 0)
        {
            return;
        }

        if (_playerManagerState == PlayerManagerState.NOT_SWAPPING)
        {
            _playerManagerState = PlayerManagerState.Swapping;
            CurrentCharacter.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.SWAPPING;
            lastCharacter = currentIndex;

            Debug.Log("You are now : " + lastCharacter + " and can swap to : " + currentIndex);
        }
    }

    /**
        * The following function selects the character to swap to
        */
    private void SelectCharacter()
    {
        if (_playerManagerState == PlayerManagerState.Swapping)
        {
            currentIndex = (currentIndex + 1) % SwappableCharacters.Count;
            //increase the opacity of the outline of the character that can be swapped to
            foreach (Image outline in UIOutlines)
            {
                if (outline == UIOutlines[currentIndex])
                {
                    outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 1f);
                }
                else
                {
                    outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 0.5f);
                }
            }

            Debug.Log("You are now : " + lastCharacter + " and can swap to : " + currentIndex);
        }
    }

    /**
        * The following function swaps the character and changes the state of the player manager
        */
    private void SwapCharacter()
    {
        if (_playerManagerState == PlayerManagerState.Swapping)
        {
            _playerManagerState = PlayerManagerState.NOT_SWAPPING;
            Debug.Log("Swapping from : " + lastCharacter + " to : " + currentIndex);
            CurrentCharacter = SwappableCharacters[currentIndex];

            //reset timer
            //swapTimer = swapTimeLimit;

            foreach (GameObject character in SwappableCharacters)
            {
                if (character != CurrentCharacter)
                {
                    character.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.SWAPPED_OUT;
                }
                else
                {
                    character.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.IDLE;
                }
            }

            SwapsLeft--;
        }
    }

    /**
        * The following function cancels the swap and changes the state of the player manager
        */
    private void CancelSwap()
    {
        if (_playerManagerState == PlayerManagerState.Swapping)
        {
            _playerManagerState = PlayerManagerState.NOT_SWAPPING;
            Debug.Log("Cancelled Swap, youw are still : " + lastCharacter);

            foreach (GameObject character in SwappableCharacters)
            {
                if (character != CurrentCharacter)
                {
                    character.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.SWAPPED_OUT;
                }
                else
                {
                    character.GetComponent<PlayerBehaviour>()._playerState = CurrentPlayerState.IDLE;
                }
            }

            currentIndex = lastCharacter;

            //reset outlines
            foreach (Image outline in UIOutlines)
            {
                if (outline == UIOutlines[currentIndex])
                {
                    outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 1f);
                }
                else
                {
                    outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 0.5f);
                }
            }
        }
    }

}
