using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBehaviour : EntityBehaviour
{

    private SokobanBehaviour sokobanScript;
    private PlayerBehaviour playerScript;
    private PlayerBehaviour player1Script;
    private Rigidbody2D player;
    private Rigidbody2D player1;
    private Rigidbody2D box;

    private Collider2D boxCollider;
    private Collider2D playerCollider;
    private Collider2D player1Collider;

    // ice parameters
    //private Vector2 _closestIce = Vector2.positiveInfinity;
    //private bool _isOnIce = false;
    //private float _iceThreshold = 0.9f;
    //private bool _lockForce = false;
    private Vector2 previousLocation;
    //private Vector2 lastPlayerLocation;

    // sound parameters
    [SerializeField] private AudioSource boxSoundSource;
    [SerializeField] private AudioClip boxGroundPush;
    //[SerializeField] private AudioClip boxIceSlide;

    private bool isOnGoal = false;

    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();

        player = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        // player1 = (Rigidbody2D)GameObject.Find("Player(1)").GetComponent("Rigidbody2D");
        playerScript = PlayerManager.Instance.CurrentCharacter.GetComponent<PlayerBehaviour>();
        // player1Script = (PlayerBehaviour)player1.gameObject.GetComponent(typeof(PlayerBehaviour));

        box = gameObject.GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().isTrigger = true;
        sokobanScript = (SokobanBehaviour)player.gameObject.GetComponent(typeof(SokobanBehaviour));

        boxCollider = box.GetComponent<Collider2D>();
    }

    //Update is called once per frame
    override public void FixedUpdate()
    {
        base.FixedUpdate();

        //updateIce();
        previousLocation = box.position;
    }

    override public Vector2 getMovement()
    {
        Vector2 update = Vector2.zero;

        //if (!_lockForce)
        //{
        //    lastPlayerLocation = playerScript.dirToVec();
        //}
        // if we are on ice, we're moving in our current direction
        //if (_isOnIce)
        //{
        //    boxSoundSource.volume = 1f;
        //    boxSoundSource.clip = boxIceSlide;
        //    if (!boxSoundSource.isPlaying)
        //    {
        //        boxSoundSource.Play();
        //    }
        //    _lockForce = true;
        //    return lastPlayerLocation;
        //}

        //if (isStopped())
        //{
        //    _lockForce = false;
        //}

        // basically checking if the box has been moved onto a pressure plate, if it has then it will not enter the loop
        if (!isOnGoal)
        {

            player = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
            playerScript = PlayerManager.Instance.CurrentCharacter.GetComponent<PlayerBehaviour>();

            //Debug.Log("Player: " + player);
            
            // handle player collision with crate
            if (Mathf.Abs(Vector2.Distance(player.position, transform.position)) <= 1f
            // || Mathf.Abs(Vector2.Distance(player1.position, transform.position)) <= 1f
            )
            {
                // check if our position matches with the player
                Vector2 position = rb.position;
                Vector2 extents = _collider.bounds.extents;

                // each of the sides for checking against the player 
                Vector2 right = new Vector2(position.x + extents.x, position.y);
                Vector2 left = new Vector2(position.x - extents.x, position.y);
                Vector2 up = new Vector2(position.x, position.y + extents.y);
                Vector2 down = new Vector2(position.x, position.y - extents.y);

                Vector2 rightDir = new Vector2(1, 0);
                Vector2 leftDir = new Vector2(-1, 0);
                Vector2 upDir = new Vector2(0, 1);
                Vector2 downDir = new Vector2(0, -1);

                // handles pushing depending on which player is pushing the box
                if ((Mathf.Abs(Vector2.Distance(player.position, transform.position)) <= 1f) && (playerScript._playerState == CurrentPlayerState.IDLE))
                { // if raccoon is pushing

                    float rightDist = Vector2.Distance(right, player.position);
                    float leftDist = Vector2.Distance(left, player.position);
                    float upDist = Vector2.Distance(up, player.position);
                    float downDist = Vector2.Distance(down, player.position);

                    float minDist = Mathf.Min(rightDist, leftDist, upDist, downDist);
                    Vector2 playerDirection = playerScript.dirToVec();

                    // according to case, check if we should be allowed to push the block!
                    if (minDist == rightDist && playerDirection == leftDir)
                    {
                        update = new Vector2(-1, 0);
                    }
                    else if (minDist == leftDist && playerScript.dirToVec() == rightDir)
                    {
                        update = new Vector2(1, 0);
                    }
                    else if (minDist == upDist && playerScript.dirToVec() == downDir)
                    {
                        update = new Vector2(0, -1);
                    }
                    else if (minDist == downDist && playerScript.dirToVec() == upDir)
                    {
                        update = new Vector2(0, 1);
                    }
                    // if we have an update, we need to be making the move sound!
                    
                    

                }
                

                // else if ((Mathf.Abs(Vector2.Distance(player1.position, transform.position)) <= 1f) && (player1Behaviour._playerState == CurrentPlayerState.IDLE))// other player is pushing the box
                // {
                //     float rightDist = Vector2.Distance(right, player1.position);
                //     float leftDist = Vector2.Distance(left, player1.position);
                //     float upDist = Vector2.Distance(up, player1.position);
                //     float downDist = Vector2.Distance(down, player1.position);

                //     float minDist = Mathf.Min(rightDist, leftDist, upDist, downDist);
                //     Vector2 player1Direction = player1Script.dirToVec();

                //     // according to case, check if we should be allowed to push the block!
                //     if (minDist == rightDist && player1Direction == leftDir)
                //     {
                //         update = new Vector2(-1, 0);
                //     }
                //     else if (minDist == leftDist && player1Script.dirToVec() == rightDir)
                //     {
                //         update = new Vector2(1, 0);
                //     }
                //     else if (minDist == upDist && player1Script.dirToVec() == downDir)
                //     {
                //         update = new Vector2(0, -1);
                //     }
                //     else if (minDist == downDist && player1Script.dirToVec() == upDir)
                //     {
                //         update = new Vector2(0, 1);
                //     }
                // }
            }
            if (previousLocation != (Vector2)transform.position)
            {
                //Debug.Log("making moving sound");
                boxSoundSource.clip = boxGroundPush;
                boxSoundSource.volume = 1f;
                if (!boxSoundSource.isPlaying)
                {
                    boxSoundSource.Play();
                }
            }
            // if the box is stationary, stop all sound!
            else
            {
                //Debug.Log("stopping moving sound");
                boxSoundSource.Stop();
                boxSoundSource.volume = Mathf.Clamp(boxSoundSource.volume - 0.1f, 0f, 1f);
                if (boxSoundSource.volume == 0f)
                {
                    boxSoundSource.Stop();
                }
            }
        }
        
        return update;
    }

    //Just overlapped a collider 2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            // stopping the box at the goal/pressure plate
            box.velocity = Vector2.zero;
            boxCollider.isTrigger = false; // changing the trigger so that the box wont be moved again
            box.isKinematic = true;

            isOnGoal = true;

            sokobanScript.DecrementGoals();
            sokobanScript.DecrementCrates();

            //Destroy(collision.gameObject);
            //Destroy(gameObject);
            IEnumerator coroutine = MoveSelfToPosition(collision.transform.position);
            StartCoroutine(coroutine);

            CameraManager.Instance.ShakeCamera(0.2f, 0.1f);
            Debug.Log("stopping moving sound");
            boxSoundSource.Stop();
            boxSoundSource.volume = Mathf.Clamp(boxSoundSource.volume - 0.1f, 0f, 1f);
            if (boxSoundSource.volume == 0f)
            {
                boxSoundSource.Stop();
            }
        }
    }

    private IEnumerator MoveSelfToPosition(Vector2 position)
    {
        while (Vector2.Distance(transform.position, position) > 0.01f)
        {
            transform.position = Vector2.Lerp(transform.position, position, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        yield return null;

    }

    //public void updateIce()
    //{
    //    Vector2 icePos;

    //    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ice"))
    //    {
    //        icePos = go.transform.position;
    //        if (Mathf.Abs(Vector2.Distance(transform.position, icePos)) < Mathf.Abs(Vector2.Distance(transform.position, _closestIce)))
    //        {
    //            _closestIce = icePos;
    //        }
    //    }

    //    if (Mathf.Abs(_closestIce.x - transform.position.x) <= _iceThreshold && Mathf.Abs(_closestIce.y - transform.position.y) <= _iceThreshold && !isStopped())
    //    {

    //        _isOnIce = true;
    //    }
    //    else
    //    {
    //        _isOnIce = false;
    //    }
    //}

    public bool isStopped()
    {
        return (box.position.x == previousLocation.x && box.position.y == previousLocation.y);
    }
}
