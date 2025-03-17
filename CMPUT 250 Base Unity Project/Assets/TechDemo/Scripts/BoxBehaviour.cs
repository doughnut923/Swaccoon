using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class BoxBehaviour : EntityBehaviour
{

    protected SokobanBehaviour sokobanScript;
    protected PlayerBehaviour playerScript;
    protected PlayerBehaviour player1Script;
    protected Rigidbody2D player;
    protected Rigidbody2D player1;
    protected Rigidbody2D box;

    protected Collider2D boxCollider;
    protected Collider2D playerCollider;
    protected Collider2D player1Collider;

    // ice parameters
    //protected Vector2 _closestIce = Vector2.positiveInfinity;
    //protected bool _isOnIce = false;
    //protected float _iceThreshold = 0.9f;
    //protected bool _lockForce = false;
    protected Vector2 previousLocation;
    protected Vector3 TargetPosition;
    protected Vector3 StartPosition;
    
    private Vector3 SpawnPosition;
    private bool isSinking = false;

    //protected Vector2 lastPlayerLocation;

    // sound parameters
    [SerializeField] protected AudioSource boxSoundSource;
    [SerializeField] protected AudioClip boxGroundPush;
    [SerializeField] SpriteRenderer spriteRenderer;
    //[SerializeField] protected AudioClip boxIceSlide;

    [SerializeField] private AudioSource boxSinkingSoundSource;
    [SerializeField] private AudioClip boxSinkingPulledSound;

    [SerializeField] private float spawnRaise = 10f;
    [SerializeField] private float padding = 1.5f;

    protected bool isOnGoal = false;

    public bool isOnWater = false;

    // Start is called before the first frame update

    public void Awake(){
        SpawnPosition = transform.position;
    }

    override public void Start()
    {
        base.Start();

        player = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        // player1 = (Rigidbody2D)GameObject.Find("Player(1)").GetComponent("Rigidbody2D");
        playerScript = PlayerManager.Instance.CurrentCharacter.GetComponent<PlayerBehaviour>();
        // player1Script = (PlayerBehaviour)player1.gameObject.GetComponent(typeof(PlayerBehaviour));

        box = gameObject.GetComponent<Rigidbody2D>();
        // GetComponent<Collider2D>().isTrigger = true;
        sokobanScript = (SokobanBehaviour)player.gameObject.GetComponent(typeof(SokobanBehaviour));

        boxCollider = box.GetComponent<Collider2D>();

        //TargetPosition = transform.position;
        //transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
        //StartPosition = transform.position;
        //spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    //Update is called once per frame
    override public void FixedUpdate()
    {
        base.FixedUpdate();

        //updateIce();
        previousLocation = box.position;

        // if the box is on a water tile, it will sink
        if (isOnWater && !isSinking)
        {
            isSinking = true;
            StartCoroutine(SinkBox());
        }
    }

    IEnumerator SinkBox(){

        //Play the box sinking sound

        //Play the box sinking animation

        //Wait for the animation to finish
        Debug.Log("sinking box");
        


        //Destroy the box
        //Destroy(gameObject);
        boxSinkingSoundSource.clip = boxSinkingPulledSound;
        boxSinkingSoundSource.volume = 0.5f;
        boxSinkingSoundSource.Play();

        //Player Box sinking animation

        //called After box sinking animation is done
        yield return new WaitForSeconds(1.0f);

        //Spawn a new box in the box spawnpoint
        box.position = new Vector2(SpawnPosition.x, SpawnPosition.y);
        GetComponent<Animator>().Play("SpawnBox");
        isOnWater = false;
        boxCollider.isTrigger = true;
        isSinking = false;

        //spriteRenderer.color = new Color(1, 1, 1, 0);
        yield return null;
        //while (transform.position.y > WaterBehaviour.transform.position.y)
        //{
        //    transform.position = Vector2.Lerp(transform.position, pitPosition, 0.1f);
        //    yield return new WaitForFixedUpdate();
        //}
    }

    override public void moveAndCollide(Vector2 update)
    {
        // if we aren't moving, don't move!
        if (update == Vector2.zero) { return; }

        // move, with respect for collisions
        Vector2 position = rb.position;
        Vector2 extents = _collider.bounds.extents;
        Vector2 flush = Vector2.zero;

        // corners for raycasting
        Vector2 topRight = new Vector2(position.x + extents.x * padding, position.y + extents.y * padding);
        Vector2 topLeft = new Vector2(position.x - extents.x * padding, position.y + extents.y * padding);
        Vector2 bottomRight = new Vector2(position.x + extents.x * padding, position.y - extents.y * padding);
        Vector2 bottomLeft = new Vector2(position.x - extents.x * padding, position.y - extents.y * padding);

        // perform 2 raycasts in total from appropriate corners to check collision
        // raycasting is used due to issues with tilemap colliders

        //    ^  ^
        //    |  |
        // <- *--* ->
        //    |  |
        // <- *--* ->
        //    |  |
        //    V  V

        // horizontal, right
        if (update.x > 0)
        {
            RaycastHit2D tr = Physics2D.Raycast(topRight, Vector2.right);
            RaycastHit2D br = Physics2D.Raycast(bottomRight, Vector2.right);

            if (collided(tr) || collided(br))
            {
                update.x = 0;
                if (collided(tr))
                {
                    flush.x = tr.distance;
                }
                if (collided(br))
                {
                    flush.x = br.distance;
                }
            }
        }
        // horizontal, left
        else if (update.x < 0)
        {
            RaycastHit2D tl = Physics2D.Raycast(topLeft, Vector2.left);
            RaycastHit2D bl = Physics2D.Raycast(bottomLeft, Vector2.left);

            if (collided(tl) || collided(bl))
            {
                update.x = 0;
                if (collided(tl))
                {
                    flush.x = tl.distance;
                }
                if (collided(bl))
                {
                    flush.x = bl.distance;
                }
            }
        }

        // vertical, up
        if (update.y > 0)
        {
            RaycastHit2D tr = Physics2D.Raycast(topRight, Vector2.up);
            RaycastHit2D tl = Physics2D.Raycast(topLeft, Vector2.up);

            if (collided(tr) || collided(tl))
            {
                update.y = 0;
                if (collided(tr))
                {
                    flush.y = tr.distance;
                }
                if (collided(tl))
                {
                    flush.y = tl.distance;
                }
            }
        }
        // vertical, down
        else if (update.y < 0)
        {
            RaycastHit2D br = Physics2D.Raycast(bottomRight, Vector2.down);
            RaycastHit2D bl = Physics2D.Raycast(bottomLeft, Vector2.down);

            if (collided(br) || collided(bl))
            {
                update.y = 0;
                if (collided(br))
                {
                    flush.y = br.distance;
                }
                if (collided(bl))
                {
                    flush.y = bl.distance;
                }
            }
        }

        // update position and clamp according to flush raycasts
        Vector2 newPosition = position + update * moveSpeed * Time.deltaTime;

        if (flush.x != 0)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, position.x - flush.x, position.x + flush.x);
        }
        if (flush.y != 0)
        {
            newPosition.y = Mathf.Clamp(newPosition.y, position.y - flush.y, position.y + flush.y);
        }

        rb.MovePosition(newPosition);
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
    virtual protected void OnTriggerEnter2D(Collider2D collision)
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

            GameObject goalGameObject = collision.gameObject;
            if(goalGameObject.GetComponent<GoalBehaviour>() != null)
            {
                goalGameObject.GetComponent<GoalBehaviour>().onGoalReached();
            }

            //Destroy(collision.gameObject);
            //Destroy(gameObject);
            IEnumerator coroutine = MoveSelfToPosition(collision.transform.position);
            StartCoroutine(coroutine);

            CameraManager.Instance.ShakeCamera(0.2f, 0.1f);
            // checks if we won everytime we move box onto goal
            //sokobanScript.Win();
            Debug.Log("stopping moving sound");
            boxSoundSource.Stop();
            boxSoundSource.volume = Mathf.Clamp(boxSoundSource.volume - 0.1f, 0f, 1f);
            if (boxSoundSource.volume == 0f)
            {
                boxSoundSource.Stop();
            }
        }
    }

    protected IEnumerator MoveSelfToPosition(Vector2 position)
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
