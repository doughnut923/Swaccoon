using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//used https://medium.com/@eveciana21/creating-a-title-screen-and-a-new-game-button-in-unity-d1ef6c1f8f84#:~:text=Simply%20right%20click%20on%20your,Aspect%20to%20maintain%20its%20scaling.

public class TitlePage : MonoBehaviour
{

    public void LoadGame()
    {
        // loads the first game scene --> would be intro tutorial
        SceneManager.LoadScene("intro"); // currently loads to fish intro
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
