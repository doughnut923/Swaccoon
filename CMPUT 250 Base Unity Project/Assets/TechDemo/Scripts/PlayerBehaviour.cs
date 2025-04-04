﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public enum CurrentPlayerState
{
    IDLE,
    WALKING,
    FALLING,
    ATTACKING,
    SWAPPING,
    SWAPPED_OUT,
    CUTSCENE_PLAYING
}

/***
Basically a PlayerBehaviour will have the states above, and only one of them will be active at a time.
The inactive character will be the SWAPPED_OUT state, and the active character will be one of the others.
The switching between inactive and active states of characters will be controlled by the PlayerManager
And the switching between the active states will be controlled by the PlayerBehaviour itself.
***/

public class PlayerBehaviour : EntityBehaviour
{

    public CurrentPlayerState _playerState;


    [Header("Input Bindings")]
    [SerializeField] public KeyCode left = KeyCode.LeftArrow;
    [SerializeField] public KeyCode right = KeyCode.RightArrow;
    [SerializeField] public KeyCode up = KeyCode.UpArrow;
    [SerializeField] public KeyCode down = KeyCode.DownArrow;
    [SerializeField] public KeyCode attack = KeyCode.Space;
    [SerializeField] public KeyCode reset = KeyCode.R;
    [SerializeField] public KeyCode swap = KeyCode.E;
    [SerializeField] public KeyCode punch = KeyCode.A;

    // public variables
    public float attackCooldown = 2f;

    [Header("Sprite Settings")]
    [SerializeField] protected List<Sprite> idleSpriteUp = new List<Sprite>(6);
    [SerializeField] protected List<Sprite> walkSpritesUp = new List<Sprite>(6);
    [SerializeField] protected List<Sprite> idleSpriteRight = new List<Sprite>(6);
    [SerializeField] protected List<Sprite> walkSpritesRight = new List<Sprite>(6);
    [SerializeField] protected List<Sprite> idleSpriteDown = new List<Sprite>(6);
    [SerializeField] protected List<Sprite> walkSpritesDown = new List<Sprite>(6);
    [SerializeField] protected List<Sprite> idleSpriteLeft = new List<Sprite>(6);
    [SerializeField] protected List<Sprite> walkSpritesLeft = new List<Sprite>(6);

    [SerializeField] protected List<Sprite> attackSpritesUp = new List<Sprite>(4);
    [SerializeField] protected List<Sprite> attackSpritesRight = new List<Sprite>(4);
    [SerializeField] protected List<Sprite> attackSpritesDown = new List<Sprite>(4);
    [SerializeField] protected List<Sprite> attackSpritesLeft = new List<Sprite>(4);

    [SerializeField] protected List<Sprite> fallSprites = new List<Sprite>(6);

    [SerializeField] protected List<Sprite> punchSpritesUp = new List<Sprite>(4);
    [SerializeField] protected List<Sprite> punchSpritesDown = new List<Sprite>(4);
    [SerializeField] protected List<Sprite> punchSpritesLeft = new List<Sprite>(4);
    [SerializeField] protected List<Sprite> punchSpritesRight = new List<Sprite>(4);

    [SerializeField] protected List<Sprite> sleepSpritesUp = new List<Sprite>(4);
    [SerializeField] protected List<Sprite> sleepSpritesDown = new List<Sprite>(4);
    [SerializeField] protected List<Sprite> sleepSpritesLeft = new List<Sprite>(4);
    [SerializeField] protected List<Sprite> sleepSpritesRight = new List<Sprite>(4);

    [SerializeField] protected Sprite hurtSpriteUp;
    [SerializeField] protected Sprite hurtSpriteRight;
    [SerializeField] protected Sprite hurtSpriteDown;
    [SerializeField] protected Sprite hurtSpriteLeft;

    [Header("Sound Settings")]
    [SerializeField] protected AudioSource playerSoundSource;
    [SerializeField] protected AudioClip attackSound;
    [SerializeField] protected AudioClip hurtSound;
    [SerializeField] protected AudioClip deathSound;

    [SerializeField] protected AudioSource controlSoundSource;
    [SerializeField] protected AudioClip doorOpenSound;
    [SerializeField] protected AudioClip keyPickupSound;
    [SerializeField] protected AudioClip pitFallSound;

    [SerializeField] protected AudioSource playerWalkSoundSource;
    [SerializeField] protected AudioClip walkSound;
    [SerializeField] protected AudioClip slipSound;

    [SerializeField] protected Tilemap tilemap;

    public ParticleSystem walkParticles;

    // internal variables    
    // attack parameters
    protected bool _isAttacking = false;
    protected float _attackThreshold = 1.8f;
    protected float _attackCountdown;

    // punch parameters
    //protected bool _isPunching = false;
    protected float _PunchThreshold = 1.8f;
    protected float _PunchCountdown;

    // animation speed
    protected float _walkFramesPerSecond = 12.5f;
    protected float _attackFramesPerSecond = 15f;
    protected float _fallFramesPerSecond = 15f;
    //protected float _currentFrame = 0f;
    public float _currentFrame = 0f;

    // falling parameters
    protected bool _isFalling = false;
    public Vector3 lastSafePosition = Vector3.zero;

    // ice parameters
    protected Vector2 _closestIce = Vector2.positiveInfinity;
    protected bool _isOnIce = false;
    protected float _iceThreshold = 0.9f;

    // hurt parameters
    protected float _hurtFrames = 24f;
    protected float _currentHurtFrame = 0f;
    protected bool _isHurt = false;
    protected bool _isDead = false;

    // components
    protected SpriteRenderer currentSprite;
    protected SpriteRenderer shadow;

    // key parameters
    protected int _keys = 0;

    public Vector2 movement = Vector2.zero;
    protected bool _canStep = true;
    
    //add a slider to inspector to adjust acceleration
    [Header("Juice Settings")]
    [Range(0.01f, 1f)]
    [Tooltip("How fast the player accelerates")]
    [SerializeField] protected float acceleration = 0.1f;
    protected Vector2 currentSpeed = Vector2.zero;
    [Range(0f, 45f)]
    [SerializeField] protected float TiltAngle = 10f;
    protected float currentTilt = 0f;
    [Range(0.01f, 1f)]
    [SerializeField] protected float tiltSpeed = 0.2f;
    [SerializeField] protected float particleTimer = 0.5f;

    public bool isPunching = false;
    public bool inWater = false;
    public float original_z = 0f;

    private Rigidbody2D playerManager;

    // Start is called before the first frame update

    void Awake()
    {
        original_z  = transform.position.z;
    }
    override public void Start()
    {
        base.Start();
        
        _attackCountdown = attackCooldown;

        // reset the room if we aren't using the connective wrapper
        if (SceneManager.sceneCount == 1)
        {
            PlayerPrefs.DeleteAll();
        }

        if (!PlayerPrefs.HasKey("keys"))
        {
            PlayerPrefs.SetInt("keys", 0);
        }
        if (PlayerPrefs.HasKey("health"))
        {
            _health = PlayerPrefs.GetInt("health");
        }

        // set up sprite
        currentSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        shadow = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();

        // set up walk sound
        playerWalkSoundSource.clip = walkSound;

        lastSafePosition = transform.position;
    }

    // Update is called once per frame
    override public void FixedUpdate()
    {   
        if(GameStateManager.instance.gameState == GameState.GAME_OVER)
        {
            return;
        }

        //Debug.Log("At 1");
        if(_playerState == CurrentPlayerState.CUTSCENE_PLAYING)
        {
            //Possibly sleepping animation
            //but wil be just idle for now
            handleAnimation();
            return;

        }

        //Debug.Log("At 2");

        CutSceneManager cm = CutSceneManager.instance;
        if(cm != null && !CutSceneManager.instance.canMove){
            //Possibly sleepping animation
            //but wil be just idle for now
            DoIdleAnimation();
            return;
        }

        //Debug.Log("At 3");
        if (_playerState == CurrentPlayerState.SWAPPED_OUT)
        {

            //Possibly sleepping animation
            //but wil be just idle for now
            currentSprite.color = new Color(0.5f, 0.5f, 0.5f, 1f); // change color to grey if swapped out or if cutscene dialogue is playing (maybe set as variable)
            DoIdleAnimation();
            return;
        }
        else
        {
            currentSprite.color = new Color(1, 1, 1, 1); // change color back to original (maybe set as variable)
        }

        base.FixedUpdate();

        if(gameObject.name == "Fish"){
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

        if (_isDead)
        {
            // only die once we are done playing the death sound!
            if (playerSoundSource.isPlaying) { return; }

            // get rid of health and keys on death, and reload the scene
            PlayerPrefs.SetInt("health", 3);
            PlayerPrefs.SetInt("keys", 0);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //Destroy(gameObject);
        }
    }

    public void LateUpdate(){
        UpdateSafePosition();
    }

    public void UndoMove(){
        transform.position = lastSafePosition;
    }


    override public Vector2 getMovement()
    {
        Vector2 update = Vector2.Lerp(currentSpeed, Vector2.zero, acceleration);
        // if we are attacking or falling or hurt or dead (i wish i used a state machine), we're frozen

        // // if we are on ice, we're moving in our current direction
        // if (_isOnIce) { return dirToVec(); }

        // read input, and set our direction accordingly
        //playerManager = PlayerManager.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        //playerManager = FishBehaviour.Instance.CurrentCharacter.GetComponent<Rigidbody2D>();
        //Debug.Log(transform.position);
        if (Input.GetKey(right))
        {
            update.x = Mathf.Lerp(currentSpeed.x, 1, acceleration);
            _currDir = Direction.East;
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

        currentSpeed = update;

        // If not moving set the player tilt to 0
        currentTilt = Mathf.Lerp(currentTilt, 0, tiltSpeed);
        transform.rotation = Quaternion.Euler(0, 0, currentTilt);

        _playerState = CurrentPlayerState.IDLE;
        return update;
    }
    
    protected IEnumerator WaitForNextStep()
    {
        yield return new WaitForSeconds(particleTimer);
        _canStep = true;
    }

    public void DoFallAnimation()
    {
        if (_isFalling)
        {
            Debug.Log("_isfalling is set to " + _isFalling);
            int lastFrame = Mathf.FloorToInt(_currentFrame);
            _currentFrame = Mathf.Repeat(_currentFrame + Time.deltaTime * _fallFramesPerSecond, 6f);

            // we are done falling! set our position accordingly and take some damage
            if (lastFrame == 5 && Mathf.FloorToInt(_currentFrame) == 0)
            {
                _isFalling = false;
                setInvincible(false);
                transform.position = lastSafePosition;
                takeDamage();
            }
            // keep playing the animation otherwise
            else
            {
                currentSprite.sprite = fallSprites[Mathf.FloorToInt(_currentFrame)];
                return;
            }
        }
    }

    public void DoIdleAnimation()
    {
        _currentFrame = Mathf.Repeat(_currentFrame + Time.deltaTime * _walkFramesPerSecond, 6f);
        //Debug.Log("current sprite is");
        //switch (_currDir)
        //{
        //    case Direction.North:
        //        currentSprite.sprite = idleSpriteUp[Mathf.FloorToInt(_currentFrame)];
        //        break;
        //    case Direction.East:
        //        currentSprite.sprite = idleSpriteRight[Mathf.FloorToInt(_currentFrame)];
        //        break;
        //    case Direction.South:
        //        currentSprite.sprite = idleSpriteDown[Mathf.FloorToInt(_currentFrame)];
        //        break;
        //    case Direction.West:
        //        currentSprite.sprite = idleSpriteLeft[Mathf.FloorToInt(_currentFrame)];
        //        break;
        //    default:
        //        break;
        //}

        switch (_currDir)
        {
            case Direction.North:
                currentSprite.sprite = sleepSpritesUp[Mathf.FloorToInt(_currentFrame)];
                break;
            case Direction.East:
                currentSprite.sprite = sleepSpritesRight[Mathf.FloorToInt(_currentFrame)];
                break;
            case Direction.South:
                currentSprite.sprite = sleepSpritesDown[Mathf.FloorToInt(_currentFrame)];
                break;
            case Direction.West:
                currentSprite.sprite = sleepSpritesLeft[Mathf.FloorToInt(_currentFrame)];
                break;
            default:
                break;
        }
    }

    public static Direction vectTorDir(Vector2 v){
        //Normalize the vector to get the direction;
        if (v.x > 0 && v.y > 0){
            return Direction.East;
        }
        else if (v.x > 0 && v.y < 0){
            return Direction.East;
        }
        else if (v.x < 0 && v.y > 0){
            return Direction.West;
        }
        else if (v.x < 0 && v.y < 0){
            return Direction.West;
        }
        else if (v.x > 0){
            return Direction.East;
        }
        else if (v.x < 0){
            return Direction.West;
        }
        else if (v.y > 0){
            return Direction.North;
        }
        else if (v.y < 0){
            return Direction.South;
        }
        return Direction.North;
    }

    public virtual void UpdateSafePosition()
    {
        //check whether the tile that the player standing on is a safe tile (i.e. Ground Tile), if so update the last safe position
        Vector3Int gridPosition = tilemap.WorldToCell(new Vector3(transform.position.x, transform.position.y, transform.position.z));
        TileBase searchedTile = tilemap.GetTile(gridPosition);

        
        
        if(searchedTile == null){
            return;
        }

        if(!searchedTile.name.Contains("Water") && !searchedTile.name.ToLower().Contains("pit")){
            // Debug.Log("searched tile is " + searchedTile.name + ", Setting last safe position to " + searchedTile.name);    
            Vector3 worldPos = tilemap.CellToWorld(gridPosition);
            lastSafePosition = new Vector3(worldPos.x + tilemap.cellSize.x/2, worldPos.y + tilemap.cellSize.y/2, original_z);
        }
        // if(!_isFalling){
        //     //set the safe position to the center of the tile
        //     Vector3Int gridPosition = tilemap.WorldToCell(new Vector3(transform.position.x, transform.position.y, transform.position.z));
        //     Vector3 worldPos = tilemap.CellToWorld(gridPosition);
        //     lastSafePosition = new Vector2(worldPos.x + tilemap.cellSize.x/2, worldPos.y + tilemap.cellSize.y/2);
        // }
    }

    //public void DoPunchAnimation()
    //{
    //    //_playerState = CurrentPlayerState.ATTACKING;
    //    _currentFrame = Mathf.Repeat(_currentFrame + Time.deltaTime * _attackFramesPerSecond, 4f);
    //    //Debug.Log(_playerState);

    //    //Debug.Log("current sprite is " + currentSprite);
    //    //Debug.Log("current frame is " + _currentFrame);
    //    //Debug.Log("punch now" + punchSpritesUp[Mathf.FloorToInt(_currentFrame)]);
    //    //Debug.Log("")
    //    switch (_currDir)
    //    {
    //        case Direction.North:
    //            //Debug.Log("punch now" + punchSpritesUp[Mathf.FloorToInt(_currentFrame)]);
    //            currentSprite.sprite = punchSpritesUp[Mathf.FloorToInt(_currentFrame)];
    //            //Debug.Log("sprite is " + currentSprite.sprite);
    //            break;
    //        case Direction.East:
    //            currentSprite.sprite = punchSpritesRight[Mathf.FloorToInt(_currentFrame)];
    //            break;
    //        case Direction.South:
    //            currentSprite.sprite = punchSpritesDown[Mathf.FloorToInt(_currentFrame)];
    //            break;
    //        case Direction.West:
    //            currentSprite.sprite = punchSpritesLeft[Mathf.FloorToInt(_currentFrame)];
    //            break;
    //        default:
    //            break;
    //    }
    //    if (_currentFrame >= 4f - 1)
    //    {
    //        isPunching = false;
    //    }
    //}

    public void handleAnimation()
    {


        // Debug.Log("From " + name + " " +  movement.x + " " + movement.y);
        // // if we are falling, do the falling animation lol
        
        //Debug.Log("player is currently " + currentSprite);
        if (_isFalling){
            // Debug.Log("_isfalling is set to " + _isFalling);
            int lastFrame = Mathf.FloorToInt(_currentFrame);
            _currentFrame = Mathf.Repeat(_currentFrame + Time.deltaTime * _fallFramesPerSecond, 6f);

            // we are done falling! set our position accordingly and take some damage
            if (lastFrame == 5 && Mathf.FloorToInt(_currentFrame) == 0){
                _isFalling = false;
                setInvincible(false);
                transform.position = lastSafePosition;
                takeDamage();
            }
            // keep playing the animation otherwise
            else{
                currentSprite.sprite = fallSprites[Mathf.FloorToInt(_currentFrame)];
                return;
            }
        }

        // otherwise, update according to current movement

        // should be idle if no movement --> including punching
        // set idle sprite based on direction

        // if punching
        if (isPunching == true)
        {
            _currentFrame = Mathf.Repeat(_currentFrame + Time.deltaTime * _attackFramesPerSecond, 4f);
            switch (_currDir)
            {
                case Direction.North:
                    currentSprite.sprite = punchSpritesUp[Mathf.FloorToInt(_currentFrame)];
                    break;
                case Direction.East:
                    currentSprite.sprite = punchSpritesRight[Mathf.FloorToInt(_currentFrame)];
                    break;
                case Direction.South:
                    currentSprite.sprite = punchSpritesDown[Mathf.FloorToInt(_currentFrame)];
                    break;
                case Direction.West:
                    currentSprite.sprite = punchSpritesLeft[Mathf.FloorToInt(_currentFrame)];
                    break;
                default:
                    break;
            }
            // sets isPunching back to false after the animation finishes
            if (_currentFrame >= 4f - 1)
            {
                isPunching = false;
            }
        }
        // if idle
        else if (((movement.x == 0 && movement.y == 0) || _isOnIce) && isPunching==false)
        {
            _playerState = CurrentPlayerState.IDLE;

            _currentFrame = Mathf.Repeat(_currentFrame + Time.deltaTime * _walkFramesPerSecond, 6f);
            switch (_currDir)
            {
                case Direction.North:
                    currentSprite.sprite = idleSpriteUp[Mathf.FloorToInt(_currentFrame)];
                    break;
                case Direction.East:
                    currentSprite.sprite = idleSpriteRight[Mathf.FloorToInt(_currentFrame)];
                    break;
                case Direction.South:
                    currentSprite.sprite = idleSpriteDown[Mathf.FloorToInt(_currentFrame)];
                    break;
                case Direction.West:
                    currentSprite.sprite = idleSpriteLeft[Mathf.FloorToInt(_currentFrame)];
                    break;
                default:
                    break;
            }
        }
        // otherwise we're moving, set the update accordingly!
        else
        {
            // Debug.Log("Ima walkin");
            _currentFrame = Mathf.Repeat(_currentFrame + Time.deltaTime * _walkFramesPerSecond, 6f);
            switch (_currDir)
            {
                case Direction.North:
                    currentSprite.sprite = walkSpritesUp[Mathf.FloorToInt(_currentFrame)];
                    break;
                case Direction.East:
                    currentSprite.sprite = walkSpritesRight[Mathf.FloorToInt(_currentFrame)];
                    break;
                case Direction.South:
                    currentSprite.sprite = walkSpritesDown[Mathf.FloorToInt(_currentFrame)];
                    break;
                case Direction.West:
                    currentSprite.sprite = walkSpritesLeft[Mathf.FloorToInt(_currentFrame)];
                    break;
                default:
                    break;  
            }
        }
        return;
    }

    public int getKeys()
    {
        return PlayerPrefs.GetInt("keys");
    }

    public void unlockGate(bool usedKey)
    {
        if (usedKey)
        {
            PlayerPrefs.SetInt("keys", PlayerPrefs.GetInt("keys") - 1);
        }

        // door sound
        controlSoundSource.clip = doorOpenSound;
        controlSoundSource.Play();
    }

    public void setDirection(Direction dir)
    {
        _currDir = dir;
    }

    public void collectKey()
    {
        PlayerPrefs.SetInt("keys", PlayerPrefs.GetInt("keys") + 1);
        // key sound
        controlSoundSource.clip = keyPickupSound;
        controlSoundSource.Play();
    }

    public void setVisible(bool vis)
    {
        currentSprite.enabled = vis;
        shadow.enabled = vis;
    }

    public void setMovment(Vector2 move)
    {
        movement = move;
    }

    public bool fallInPit(Vector2 pitPosition)
    {
        // if we are already falling, don't fall!
        if (_isFalling) { return false; }
        // time to fall! get rid of our sprite as we play the falling animation, and make us invulnerable!
        _isFalling = true;
        DoFallAnimation();

        setInvincible(true);

        // IEnumerator dropToPit = DropToPit(pitPosition);
        // StartCoroutine(dropToPit);
        _currentFrame = 0;

        // play falling sound
        controlSoundSource.clip = pitFallSound;
        controlSoundSource.Play();

        // we successfully fell!

        // Lose the game (for now)
        GameOverUIBehavior.instance.ShowGameOverUI();
        
        return true;
    }

    IEnumerator DropToPit(Vector2 pitPosition)
    {
        while (transform.position.y > pitPosition.y)
        {
            transform.position = Vector2.Lerp(transform.position, pitPosition, 0.1f);
            yield return new WaitForFixedUpdate();
        }
    }

    public void updateIce(Vector2 icePos)
    {
        if (Mathf.Abs(Vector2.Distance(transform.position, icePos)) < Mathf.Abs(Vector2.Distance(transform.position, _closestIce)))
        {
            //check if the ice block who called the updateIce function is closer to the player than the previous closest ice block, if so set the new closest ice block
            _closestIce = icePos;
        }

        if (Mathf.Abs(_closestIce.x - transform.position.x) <= _iceThreshold && Mathf.Abs(_closestIce.y - transform.position.y) <= _iceThreshold && !isStopped())
        {
            // To check the distance between the player and the ice block if it is less than the threshold, then the player is on ice
            _isOnIce = true;

        }
        else
        {
            // If the player is not on ice, then the player is not on ice
            _isOnIce = false;
        }
    }

    public bool isStopped()
    {
        return (lastSafePosition.x == transform.position.x && lastSafePosition.y == transform.position.y);
    }

    public void CreateWalkParticles()
    {
        walkParticles.Play();
    }

    public bool attackCollision(RaycastHit2D attackRay)
    {
        //return attackRay.collider && attackRay.distance < _attackThreshold && attackRay.transform.gameObject.tag == "Enemy";
        return attackRay.collider && attackRay.distance < _attackThreshold && attackRay.transform.gameObject.tag == "Wall";
        //return attackRay.collider && attackRay.distance < _attackThreshold && attackRay.transform.gameObject.tag == "Boat";
    }

    public override void handleDeath(){
        // GameOverUIBehavior.instance.ShowGameOverUI();
        // Destroy(gameObject);
    }
}


public class CutsceneElements{
    
}