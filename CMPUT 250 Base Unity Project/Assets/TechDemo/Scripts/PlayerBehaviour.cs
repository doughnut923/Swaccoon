using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum CurrentPlayerState
{
    IDLE,
    WALKING,
    FALLING,
    ATTACKING,
    SWAPPING,
    SWAPPED_OUT
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
    // public variables
    public float attackCooldown = 2f;

    [Header("Sprite Settings")]
    [SerializeField] private List<Sprite> idleSpriteUp = new List<Sprite>(6);
    [SerializeField] private List<Sprite> walkSpritesUp = new List<Sprite>(6);
    [SerializeField] private List<Sprite> idleSpriteRight = new List<Sprite>(6);
    [SerializeField] private List<Sprite> walkSpritesRight = new List<Sprite>(6);
    [SerializeField] private List<Sprite> idleSpriteDown = new List<Sprite>(6);
    [SerializeField] private List<Sprite> walkSpritesDown = new List<Sprite>(6);
    [SerializeField] private List<Sprite> idleSpriteLeft = new List<Sprite>(6);
    [SerializeField] private List<Sprite> walkSpritesLeft = new List<Sprite>(6);

    [SerializeField] private List<Sprite> attackSpritesUp = new List<Sprite>(4);
    [SerializeField] private List<Sprite> attackSpritesRight = new List<Sprite>(4);
    [SerializeField] private List<Sprite> attackSpritesDown = new List<Sprite>(4);
    [SerializeField] private List<Sprite> attackSpritesLeft = new List<Sprite>(4);

    [SerializeField] private List<Sprite> fallSprites = new List<Sprite>(6);

    [SerializeField] private Sprite hurtSpriteUp;
    [SerializeField] private Sprite hurtSpriteRight;
    [SerializeField] private Sprite hurtSpriteDown;
    [SerializeField] private Sprite hurtSpriteLeft;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource playerSoundSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    [SerializeField] private AudioSource controlSoundSource;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip keyPickupSound;
    [SerializeField] private AudioClip pitFallSound;

    [SerializeField] private AudioSource playerWalkSoundSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip slipSound;

    public ParticleSystem walkParticles;

    // internal variables    
    // attack parameters
    private bool _isAttacking = false;
    private float _attackThreshold = 1.8f;
    private float _attackCountdown;

    // animation speed
    private float _walkFramesPerSecond = 12.5f;
    private float _attackFramesPerSecond = 15f;
    private float _fallFramesPerSecond = 15f;
    private float _currentFrame = 0f;

    // falling parameters
    private bool _isFalling = false;
    private Vector2 lastSafePosition = Vector2.zero;

    // ice parameters
    private Vector2 _closestIce = Vector2.positiveInfinity;
    private bool _isOnIce = false;
    private float _iceThreshold = 0.9f;

    // hurt parameters
    private float _hurtFrames = 24f;
    private float _currentHurtFrame = 0f;
    private bool _isHurt = false;
    private bool _isDead = false;

    // components
    private SpriteRenderer currentSprite;
    private SpriteRenderer shadow;

    // key parameters
    private int _keys = 0;

    private Vector2 movement = Vector2.zero;
    private bool _canStep = true;
    
    //add a slider to inspector to adjust acceleration
    [Header("Juice Settings")]
    [Range(0.01f, 1f)]
    [Tooltip("How fast the player accelerates")]
    [SerializeField] private float acceleration = 0.1f;
    private Vector2 currentSpeed = Vector2.zero;
    [Range(0f, 45f)]
    [SerializeField] private float TiltAngle = 10f;
    private float currentTilt = 0f;
    [Range(0.01f, 1f)]
    [SerializeField] private float tiltSpeed = 0.2f;
    [SerializeField] private float particleTimer = 0.5f;

    // Start is called before the first frame update
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

        if (_playerState == CurrentPlayerState.SWAPPED_OUT || _playerState == CurrentPlayerState.SWAPPING)
        {
            //Possibly sleepping animation
            //but wil be just idle for now
            DoIdleAnimation();
            return;
        }

        base.FixedUpdate();
        // handle walk sound
        if (!_isFalling && !_isOnIce && lastSafePosition != (Vector2)transform.position){
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
            _playerState = CurrentPlayerState.WALKING;

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

            // play walk partciles
            if(_canStep)
            {
                CreateWalkParticles();
                playerWalkSoundSource.Play();
                _canStep = false;
                StartCoroutine(WaitForNextStep());
            }
        }
        currentSpeed = update;

        // If not moving set the player tilt to 0
        currentTilt = Mathf.Lerp(currentTilt, 0, tiltSpeed);
        transform.rotation = Quaternion.Euler(0, 0, currentTilt);

        _playerState = CurrentPlayerState.IDLE;
        return update;
    }
    
    private IEnumerator WaitForNextStep()
    {
        yield return new WaitForSeconds(particleTimer);
        _canStep = true;
    }

    public void DoIdleAnimation()
    {
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

    public void handleAnimation()
    {

        // // if we are falling, do the falling animation lol
        if (_isFalling){
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

        // should be idle, set idle sprite based on direction
        if ((movement.x == 0 && movement.y == 0) || _isOnIce)
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

    public bool fallInPit(Vector2 pitPosition)
    {
        // if we are already falling, don't fall!
        if (_isFalling) { return false; }

        // time to fall! get rid of our sprite as we play the falling animation, and make us invulnerable!
        _isFalling = true;
        setInvincible(true);

        IEnumerator dropToPit = DropToPit(pitPosition);
        StartCoroutine(dropToPit);
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
            _closestIce = icePos;
        }

        if (Mathf.Abs(_closestIce.x - transform.position.x) <= _iceThreshold && Mathf.Abs(_closestIce.y - transform.position.y) <= _iceThreshold && !isStopped())
        {
            _isOnIce = true;

        }
        else
        {
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
        return attackRay.collider && attackRay.distance < _attackThreshold && attackRay.transform.gameObject.tag == "Enemy";
    }
}
