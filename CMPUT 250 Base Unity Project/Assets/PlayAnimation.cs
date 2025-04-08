using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites = new List<Sprite>();
    [SerializeField] float frameRate = 10f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool done = false;

    void Start()
    {
        if (sprites.Count == 0)
        {
            Debug.LogWarning("No sprites assigned to the animation.");
            return;
        }
    }

    public bool CheckDone()
    {
        return done;
    }

    public void Play(){
        StartCoroutine(PlayAnimationCoroutine());
    }

    private IEnumerator PlayAnimationCoroutine()
    {
        spriteRenderer.GetComponentInParent<PlayerBehaviour>().enabled = false;
        float frameDuration = 1f / frameRate;
        int currentFrame = 0;

        while (currentFrame < sprites.Count)
        {
            spriteRenderer.sprite = sprites[currentFrame];
            currentFrame++;
            yield return new WaitForSeconds(frameDuration);
        }
        done = true;
        spriteRenderer.GetComponentInParent<PlayerBehaviour>().enabled = true;
    }
}
