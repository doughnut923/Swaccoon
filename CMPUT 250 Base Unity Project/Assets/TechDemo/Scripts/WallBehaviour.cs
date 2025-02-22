using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehaviour : MonoBehaviour
{
    //https://www.youtube.com/watch?v=jZYxls-TOZ8

    private ParticleSystem particle;
    private SpriteRenderer spriteRenderer;
    private Collider2D wallCollider;

    private PlayerBehaviour player;
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
        particle.Play();
        spriteRenderer.enabled = false;
        wallCollider.enabled = false;
        yield return new WaitForSeconds(particle.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}
