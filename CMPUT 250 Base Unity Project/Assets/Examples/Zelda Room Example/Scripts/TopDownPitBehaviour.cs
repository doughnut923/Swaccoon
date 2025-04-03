using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownPitBehaviour : MonoBehaviour
{
    // public variables
    public float fallRadius = 0.9f;

    // the player
    private Rigidbody2D player;
    private PlayerBehaviour playerScript;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        playerScript = player.gameObject.GetComponent<PlayerBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        player = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        playerScript = player.gameObject.GetComponent<PlayerBehaviour>();
        if (Mathf.Abs(player.position.x - transform.position.x) <= fallRadius && Mathf.Abs(player.position.y - transform.position.y) <= fallRadius
        && !HavePlatform()){
            Fall();
        }
    }
    
    private bool HavePlatform(){
        foreach(PlatformBehaviour platform in PlatformManager.Instance.platforms){
            //check the bounds of the platform is in the pit if so return true
            Vector2 extents = platform.GetComponent<BoxCollider2D>().bounds.extents;
            
            Vector2 flush = Vector2.zero;

            // corners for checking if the pit is safe
            Vector2 topRight = new Vector2(platform.transform.position.x + extents.x, platform.transform.position.y + extents.y);
            Vector2 topLeft = new Vector2(platform.transform.position.x - extents.x, platform.transform.position.y + extents.y);
            Vector2 bottomRight = new Vector2(platform.transform.position.x + extents.x, platform.transform.position.y - extents.y);
            Vector2 bottomLeft = new Vector2(platform.transform.position.x - extents.x, platform.transform.position.y - extents.y);

            // check if the corners are in the pit
            if (Mathf.Abs(topRight.x - transform.position.x) <= fallRadius && Mathf.Abs(topRight.y - transform.position.y) <= fallRadius){
                return true;
            }
            if (Mathf.Abs(topLeft.x - transform.position.x) <= fallRadius && Mathf.Abs(topLeft.y - transform.position.y) <= fallRadius){
                return true;
            }
            if (Mathf.Abs(bottomRight.x - transform.position.x) <= fallRadius && Mathf.Abs(bottomRight.y - transform.position.y) <= fallRadius){
                return true;
            }
            if (Mathf.Abs(bottomLeft.x - transform.position.x) <= fallRadius && Mathf.Abs(bottomLeft.y - transform.position.y) <= fallRadius){
                return true;
            }

        }
        return false;
    }

    void Fall(){
        playerScript.fallInPit(transform.position);
    }
}
