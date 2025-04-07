using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class FishBehaviour : PlayerBehaviour
{

    [SerializeField] private float OutOfWaterLimit = 10;
    private float OutOfWaterTimeLeft= 0f;
    public bool InWater = true;
    private GameObject player;
    private PlayerBehaviour playerBehaviour;
    public Transform fishTransform;
    private Vector2 punchDir;
    private GameObject closestWater;
    [SerializeField] private float LandSpeed = 1f;
    [SerializeField] private float WaterSpeed = 2f;

    private int punch_key_down;

    [Header("Fishies Sound Settings")]
    [SerializeField] protected AudioSource fishSoundSource;
    [SerializeField] protected AudioClip punchSound;

    private WallBehaviour wall;

    private static FishBehaviour _instance;
    public static FishBehaviour Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField] private RectTransform TimeBar;

    public override void Start(){
        base.Start();
        OutOfWaterTimeLeft = OutOfWaterLimit;
        fishTransform = GameObject.Find("Fish").transform;

        GameObject[] waterObjects = GameObject.FindGameObjectsWithTag("Water");
        //get the one closest to the fish
        float closestDistance = Mathf.Infinity;
        foreach (GameObject water in waterObjects){
            float distance = Vector2.Distance(transform.position, water.transform.position);
            if (distance < closestDistance){
                closestDistance = distance;
                closestWater = water;
            }
        }
        //Debug.Log("fish transform " + fishTransform.position);
    }

    override public void FixedUpdate(){
        
        base.FixedUpdate();

        if(_playerState == CurrentPlayerState.CUTSCENE_PLAYING || GameStateManager.instance.gameState == GameState.RESET_CONFIRM)
        {
            //Possibly sleepping animation
            //but wil be just idle for now
            handleAnimation();
            return;

        }

        CutSceneManager cm = CutSceneManager.instance;
        if(cm != null && !CutSceneManager.instance.canMove){
            //Possibly sleepping animation
            //but wil be just idle for now
            DoIdleAnimation();
            return;
        }


        // check if we are in water
        checkInWater();
        // if we are out of the water, decrement the timer
        moveSpeed = WaterSpeed;
        if (!InWater)
        {
            TimeBar.gameObject.SetActive(true);
            OutOfWaterTimeLeft -= Time.deltaTime;
            //Debug.Log("Out of water time left: " + OutOfWaterTimeLeft);

            //change length of time bar
            TimeBar.sizeDelta = new Vector2(OutOfWaterTimeLeft / OutOfWaterLimit * 100 , TimeBar.sizeDelta.y);

            //set the color of the Bar based on how much time is left from white to red
            TimeBar.GetComponent<Image>().color = Color.Lerp(Color.white, Color.red, 1 - OutOfWaterTimeLeft / OutOfWaterLimit);

            moveSpeed = LandSpeed;

            if (OutOfWaterTimeLeft <= 0)
            {
                //Lose the game
                GameOverUIBehavior.instance.ShowGameOverUI();
            }
        }
        else
        {
            TimeBar.gameObject.SetActive(false);
            OutOfWaterTimeLeft = OutOfWaterLimit;
        }

        player = PlayerManager.Instance.CurrentCharacter;
        playerBehaviour = GetComponent<PlayerBehaviour>();

        if (_playerState == CurrentPlayerState.SWAPPED_OUT || _playerState == CurrentPlayerState.SWAPPING || _playerState == CurrentPlayerState.CUTSCENE_PLAYING)
        {
            //Possibly sleepping animation
            //but wil be just idle for now
            DoIdleAnimation();
            return;
        }


        // handle walk sound
        if (!_isFalling && !_isOnIce && lastSafePosition != (Vector3)transform.position){
            playerWalkSoundSource.volume = 0.25f;
            playerWalkSoundSource.clip = walkSound;
            if (!playerWalkSoundSource.isPlaying){
                // Debug.Log("Playing walk sound");
                // playerWalkSoundSource.Play();
            }
        }
        // else if (_isOnIce){
        //     playerWalkSoundSource.volume = 0.25f;
        //     playerWalkSoundSource.clip = slipSound;
        //     if (!playerWalkSoundSource.isPlaying){
        //         playerWalkSoundSource.Play();
        //     }
        // }
        // else{
        //     playerWalkSoundSource.volume = Mathf.Clamp(playerWalkSoundSource.volume - 0.05f, 0f, 1f);
        //     if (playerWalkSoundSource.volume == 0f){
        //         playerWalkSoundSource.Stop();
        //     }
        // }

        // if we aren't falling, save our last safe position!
        // if (!_isFalling){
        //     lastSafePosition = transform.position;
        // }

        // reset button
        if (Input.GetKey(reset))
        {
            handleDeath();
        }

        getMovement();
        handleAnimation();

        // if (_isDead)
        // {
        //     // only die once we are done playing the death sound!
        //     if (playerSoundSource.isPlaying) { return; }

        //     // get rid of health and keys on death, and reload the scene
        //     PlayerPrefs.SetInt("health", 3);
        //     PlayerPrefs.SetInt("keys", 0);

        //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //     //Destroy(gameObject);
        // }
    }

    override public Vector2 getMovement()
    {

        Vector2 update = Vector2.Lerp(currentSpeed, Vector2.zero, acceleration);
        // if we are attacking or falling or hurt or dead (i wish i used a state machine), we're frozen

        // // if we are on ice, we're moving in our current direction
        // if (_isOnIce) { return dirToVec(); }

        // read input, and set our direction accordingly
        if (Input.GetKey(right))
        {
            update.x = Mathf.Lerp(currentSpeed.x, 1, acceleration);
            _currDir = Direction.East;
            punchDir = transform.right;
            _playerState = CurrentPlayerState.WALKING;

            movement = update;

            // tilt the player
            currentTilt = Mathf.Lerp(currentTilt, -TiltAngle, tiltSpeed);

            // play walk partciles
            if(_canStep)
            {
                CreateWalkParticles();
                playerWalkSoundSource.Play();
                _canStep = false;
                StartCoroutine(WaitForNextStep());
            }
        }
        else if (Input.GetKey(left))
        {
            update.x = Mathf.Lerp(currentSpeed.x, -1, acceleration);
            _currDir = Direction.West;
            punchDir = -transform.right;
            _playerState = CurrentPlayerState.WALKING;

            movement = update;

            //tilt the player
            currentTilt = Mathf.Lerp(currentTilt, TiltAngle, tiltSpeed);

            // play walk partciles
            if(_canStep)
            {
                CreateWalkParticles();
                playerWalkSoundSource.Play();
                _canStep = false;
                StartCoroutine(WaitForNextStep());
            }
        }
        else if (Input.GetKey(up))
        {
            update.y = Mathf.Lerp(currentSpeed.y, 1, acceleration);
            _currDir = Direction.North;
            punchDir = transform.up;
            _playerState = CurrentPlayerState.WALKING;

            movement = update;

            // play walk partciles
            if(_canStep)
            {
                CreateWalkParticles();
                playerWalkSoundSource.Play();
                _canStep = false;
                StartCoroutine(WaitForNextStep());
            }
        }
        else if (Input.GetKey(down))
        {
            update.y = Mathf.Lerp(currentSpeed.y, -1, acceleration);
            _currDir = Direction.South;
            punchDir = -transform.up;
            _playerState = CurrentPlayerState.WALKING;

            movement = update;

            // play walk partciles
            if(_canStep)
            {
                CreateWalkParticles();
                playerWalkSoundSource.Play();
                _canStep = false;
                StartCoroutine(WaitForNextStep());
            }
        }
        else{
            movement = Vector2.zero;
        }

        if (Input.GetKey(punch) && punch_key_down == 0)
        {
            punch_key_down = 1;
            // if player wants to punch
            if (playerBehaviour.isPunching != true)
            {
                _playerState = CurrentPlayerState.ATTACKING;
                playerBehaviour.isPunching = true;
                playerBehaviour._currentFrame = 0;

                fishSoundSource.clip = punchSound;
                fishSoundSource.volume = 0.5f;
                fishSoundSource.Play();

                // Set the position and direction for the raycast
                // fishTransform.position is the current position of fish character
                // punchDir is the direction that we are punching
                // _attackThreshold is the distance the ray should travel
                RaycastHit2D attackRay = Physics2D.Raycast(fishTransform.position, punchDir, _attackThreshold);

                // Call the attackCollision function with the raycast hit result
                if (attackCollision(attackRay))
                {
                    // If attackCollision returns true, it means we hit an enemy
                    // get the collider information and checks if it is has wall behaviour script
                    WallBehaviour wall = attackRay.collider.GetComponent<WallBehaviour>();

                    Debug.Log("Attack hit something!");
                    wall.beginWallBreak();
                }
                else
                {
                    Debug.Log("did not hit anything");
                }
            }
            else
            {
                Debug.Log("cant punch, already punching");
            }

            
        }
        else if(!Input.GetKey(punch))
        {
            punch_key_down = 0;
        }
        currentSpeed = update;


        // If not moving set the player tilt to 0
        currentTilt = Mathf.Lerp(currentTilt, 0, tiltSpeed);
        transform.rotation = Quaternion.Euler(0, 0, currentTilt);
        
        // _playerState = CurrentPlayerState.IDLE;
        return update;
    }

    public override void UpdateSafePosition()
    {
        //check whether the tile that the player standing on is a safe tile (i.e. Ground Tile), if so update the last safe position
        // Vector3Int gridPosition = tilemap.WorldToCell(new Vector3(transform.position.x, transform.position.y, transform.position.z));
        // TileBase searchedTile = tilemap.GetTile(gridPosition);

        // Debug.Log(searchedTile.name);

        // if(searchedTile.name == "GroundTile"){
        //     lastSafePosition = transform.position;
        // }

        GameObject[] waterObjects = GameObject.FindGameObjectsWithTag("Water");
        //get the one closest to the fish
        float closestDistance = Mathf.Infinity;
        foreach (GameObject water in waterObjects){
            float distance = Vector2.Distance(transform.position, water.transform.position);
            if (distance < closestDistance){
                closestDistance = distance;
                closestWater = water;
            }
        }

        if(InWater){
            //set the safe position to the center of the tile
            // Vector3Int gridPosition = tilemap.WorldToCell(new Vector3(transform.position.x, transform.position.y, transform.position.z));
            // Vector3 worldPos = tilemap.CellToWorld(gridPosition);
            lastSafePosition = new Vector3 (closestWater.transform.position.x, closestWater.transform.position.y, transform.position.z);
        }
    }

    private void checkInWater(){
        //Get a list of water in scene
        GameObject[] waterObjects = GameObject.FindGameObjectsWithTag("Water");
        //get the one closest to the fish
        float closestDistance = Mathf.Infinity;
        foreach (GameObject water in waterObjects){
            float distance = Vector2.Distance(transform.position, water.transform.position);
            if (distance < closestDistance){
                closestDistance = distance;
                closestWater = water;
            }
        }
        
        if (closestWater != null)
        {
            //Debug.Log("Closest water is " + closestWater.gameObject.name);
            //compare the distnce to the water to the fish
            if (Mathf.Abs(transform.position.x - closestWater.transform.position.x) > .6 || Mathf.Abs(transform.position.y - closestWater.transform.position.y) > .6)
            {
                InWater = false;
            }
            else
            {
                InWater = true;
            }
        }
        else{
            Debug.LogWarningFormat("No water found in scene");
        }
    }

}
