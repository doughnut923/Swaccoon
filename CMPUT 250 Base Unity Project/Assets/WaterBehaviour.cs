using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    private List<BoxBehaviour> boxes = new List<BoxBehaviour>();
    private GameObject boat;
    // public variables
    public float fallRadius = 5f;

    // the player
    private Rigidbody2D player;
    private PlayerBehaviour playerScript;

    private TopDownPitBehaviour onPlatform;

    // the boat
    //private Rigidbody2D boat;

    //public Transform 

    void Start()
    {
        //get all boats in the scene
        GameObject[] boxObjects = GameObject.FindGameObjectsWithTag("Crate");

        if (boxObjects.Length == 0)
        {
            return;
        }
        foreach (GameObject boatObject in boxObjects)
        {
            boxes.Add(boatObject.GetComponent<BoxBehaviour>());
        }

        player = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        if (player == null)
        {
            Debug.LogWarning("No player in the scene");
        }  
        boat = GameObject.FindGameObjectWithTag("Boat");
        playerScript = player.gameObject.GetComponent<PlayerBehaviour>();
        //Debug.Log("fall radius " + fallRadius);

        //platform = PlatformManager.Instance.platforms;
        //platformScript = onPlatform.gameObject.GetComponent<PlatformBehaviour>;
        //GameObject fishPlayer = GameObject.Find("Fish");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("are we on the platform " + onPlatform.HavePlatform());
        //if (Mathf.Abs(player.position.x - transform.position.x) <= fallRadius && Mathf.Abs(player.position.y - transform.position.y) <= fallRadius
        //&& (!HaveBoat() || !onPlatform.HavePlatform()) && player.name != "Fish")
        //Debug.Log("havePlatform " + HavePlatform());

        player = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        if (Mathf.Abs(player.position.x - transform.position.x) <= fallRadius && Mathf.Abs(player.position.y - transform.position.y) <= fallRadius
        && !HaveBoat() && !HavePlatform() && player.name != "Fish")
        {
            //Debug.Log("fall radius is " + fallRadius);
            Fall();
        }

        if (boxes.Count == 0)
        {
            return;
        }

        foreach (BoxBehaviour box in boxes)
        {
            //if the box position is within the water's collider, set the boat IsOnWater to true
            if (GameStateManager.instance.gameState != GameState.GAME_OVER)
            {
                if (Vector2.Distance(transform.position, box.transform.position) < 0.5)
                {
                    Debug.Log("Box is in water");

                    if(!box.isSinking){
                        box.isOnWater = true;
                    }

                    //GameOverUIBehavior.instance.ShowGameOverUI();
                }
            }
        }
    }
    private bool HaveBoat()
    {
        if (boat != null)
        {
            //check the bounds of the platform is in the pit if so return true
            Vector2 extents = boat.GetComponent<BoxCollider2D>().bounds.extents;

            Vector2 flush = Vector2.zero;

            // corners for checking if the pit is safe
            Vector2 topRight = new Vector2(boat.transform.position.x + extents.x, boat.transform.position.y + extents.y);
            Vector2 topLeft = new Vector2(boat.transform.position.x - extents.x, boat.transform.position.y + extents.y);
            Vector2 bottomRight = new Vector2(boat.transform.position.x + extents.x, boat.transform.position.y - extents.y);
            Vector2 bottomLeft = new Vector2(boat.transform.position.x - extents.x, boat.transform.position.y - extents.y);

            // check if the corners are in the water
            if (Mathf.Abs(topRight.x - transform.position.x) <= fallRadius && Mathf.Abs(topRight.y - transform.position.y) <= fallRadius)
            {
                return true;
            }
            if (Mathf.Abs(topLeft.x - transform.position.x) <= fallRadius && Mathf.Abs(topLeft.y - transform.position.y) <= fallRadius)
            {
                return true;
            }
            if (Mathf.Abs(bottomRight.x - transform.position.x) <= fallRadius && Mathf.Abs(bottomRight.y - transform.position.y) <= fallRadius)
            {
                return true;
            }
            if (Mathf.Abs(bottomLeft.x - transform.position.x) <= fallRadius && Mathf.Abs(bottomLeft.y - transform.position.y) <= fallRadius)
            {
                return true;
            }

        }
        return false;
    }
    public bool HavePlatform()
    {
        if (PlatformManager.Instance == null)
        {
            return false;
        }
        if (PlatformManager.Instance.platforms.Count == 0)
        {
            return false;
        }

        if (PlatformManager.Instance.platforms.Count == 0)
        {
            return false;
        }
        foreach (PlatformBehaviour platform in PlatformManager.Instance.platforms)
        {
            //check the bounds of the platform is in the pit if so return true
            Vector2 extents = platform.GetComponent<BoxCollider2D>().bounds.extents;

            Vector2 flush = Vector2.zero;

            // corners for checking if the pit is safe
            Vector2 topRight = new Vector2(platform.transform.position.x + extents.x, platform.transform.position.y + extents.y);
            Vector2 topLeft = new Vector2(platform.transform.position.x - extents.x, platform.transform.position.y + extents.y);
            Vector2 bottomRight = new Vector2(platform.transform.position.x + extents.x, platform.transform.position.y - extents.y);
            Vector2 bottomLeft = new Vector2(platform.transform.position.x - extents.x, platform.transform.position.y - extents.y);

            // check if the corners are in the pit
            if (Mathf.Abs(topRight.x - transform.position.x) <= fallRadius && Mathf.Abs(topRight.y - transform.position.y) <= fallRadius)
            {
                return true;
            }
            if (Mathf.Abs(topLeft.x - transform.position.x) <= fallRadius && Mathf.Abs(topLeft.y - transform.position.y) <= fallRadius)
            {
                return true;
            }
            if (Mathf.Abs(bottomRight.x - transform.position.x) <= fallRadius && Mathf.Abs(bottomRight.y - transform.position.y) <= fallRadius)
            {
                return true;
            }
            if (Mathf.Abs(bottomLeft.x - transform.position.x) <= fallRadius && Mathf.Abs(bottomLeft.y - transform.position.y) <= fallRadius)
            {
                return true;
            }

        }
        return false;
    }

    void Fall()
    {
        //Debug.Log("ahh falling");
        
        playerScript.fallInPit(transform.position);
    }
}
