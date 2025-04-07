using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatfromTrigger : MonoBehaviour
{

    public PlatformBehaviour platform;

    public SpriteRenderer spriteRenderer;
    [SerializeField] protected List<Sprite> pressurePlateSprite = new List<Sprite>(2);
    protected SpriteRenderer currentSprite;

    private void Start()
    {
        currentSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

    //when player enters the trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("entering trigger zone");
        if (other.CompareTag("Player"))
        {
            //Debug.Log("game object name is " + gameObject.name);
            if (gameObject.tag == "Platform Trigger")
            {
                // start to raise platform
                platform.Rise();
            }

        }
    }
}
