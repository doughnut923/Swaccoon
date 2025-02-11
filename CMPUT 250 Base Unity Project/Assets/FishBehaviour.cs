using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class FishBehaviour : PlayerBehaviour
{

    [SerializeField] private float OutOfWaterLimit = 10;
    private float OutOfWaterTimeLeft= 0f;

    public bool InWater = true;

    [SerializeField] private Collider2D waterCollider;
    

    public override void Start(){
        base.Start();

        OutOfWaterTimeLeft = OutOfWaterLimit;
    }

    override public void FixedUpdate(){
        
        base.FixedUpdate();

        if (_playerState == CurrentPlayerState.SWAPPED_OUT || _playerState == CurrentPlayerState.SWAPPING || _playerState == CurrentPlayerState.CUTSCENE_PLAYING)
        {
            //Possibly sleepping animation
            //but wil be just idle for now
            DoIdleAnimation();
            return;
        }

        // check if we are in water
        checkInWater();
        // if we are out of the water, decrement the timer
        if (!InWater)
        {
            OutOfWaterTimeLeft -= Time.deltaTime;
            Debug.Log("Out of water time left: " + OutOfWaterTimeLeft);
            if (OutOfWaterTimeLeft <= 0)
            {
                handleDeath();
            }
        }
        else
        {
            OutOfWaterTimeLeft = OutOfWaterLimit;
        }

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

    private void checkInWater(){
        //Get a list of water in scene
        GameObject[] waterObjects = GameObject.FindGameObjectsWithTag("Water");
        //get the one closest to the fish
        GameObject closestWater = null;
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
            Debug.Log("Closest water is " + closestWater.gameObject.name);
            //compare the distnce to the water to the fish
            if (Vector2.Distance(transform.position, closestWater.transform.position) > 0.5)
            {
                InWater = false;
            }
            else
            {
                InWater = true;
            }
        }
        else{
            Debug.LogError("No water found in scene");
        }
    }

}
