using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WallBehaviour : MonoBehaviour
{
    //https://www.youtube.com/watch?v=jZYxls-TOZ8

    private ParticleSystem particle;
    private SpriteRenderer spriteRenderer;
    private Collider2D wallCollider;

    public UnityEvent onWallBreak;

    private PlayerBehaviour player;

    [SerializeField] private AudioSource wallBreakSoundSource;
    [SerializeField] private AudioClip wallBreakPulledSound;
    /// <summary>
    /// make sure to add audio!
    /// </summary>

    private void Awake()
    {
        // initialize these components once awake
        particle = GetComponentInChildren<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        wallCollider = GetComponent<Collider2D>();
        
    }

    public void beginWallBreak()
    {
        StartCoroutine(BrakeWall());
    }

    private IEnumerator BrakeWall()
    {
        // completes each item before moving to the next
        // so will finish playing the particle effect before destroying the game object wall
        wallBreakSoundSource.clip = wallBreakPulledSound;
        wallBreakSoundSource.volume = 0.5f;
        wallBreakSoundSource.Play();
        CameraManager.Instance.ShakeCamera(0.1f, 0.1f);
        particle.Play();
        spriteRenderer.enabled = false;
        wallCollider.enabled = false;
        onWallBreak?.Invoke();
        yield return new WaitForSeconds(particle.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}
