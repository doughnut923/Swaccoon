using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedEntity : MonoBehaviour
{
    public List<Sprite> DefaultAnimationCycle;
    public float Framerate = 12f;//frames per second
    public SpriteRenderer SpriteRenderer;//spriteRenderer

    //private animation stuff
    private float animationTimer;//current number of seconds since last animation frame update
    private float animationTimerMax;//max number of seconds for each frame, defined by Framerate
    protected int index;//current index in the DefaultAnimationCycle


    //Set up logic for animation stuff
    protected void AnimationSetup()
    {
        animationTimerMax = 1.0f / ((float)(Framerate));
        index = 0;
    }

    //Default animation update
    protected void AnimationUpdate()
    {
        animationTimer += Time.deltaTime;
        if (animationTimer > animationTimerMax)
        {
            animationTimer = 0;
            index++;

            if (DefaultAnimationCycle.Count == 0 || index >= DefaultAnimationCycle.Count)
            {
                index = 0;
            }
            if (DefaultAnimationCycle.Count > 0)
            {
                SpriteRenderer.sprite = DefaultAnimationCycle[index];
            }

        }
    }
}
