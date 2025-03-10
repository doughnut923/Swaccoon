using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainWave : MonoBehaviour
{

    private float currentFrame;
    [SerializeField] private List<Sprite> brainWaves = new List<Sprite>(8); 
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float animationSpeed = 0.05f;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.enabled = false;  
    }

    public void TriggerBrainWave()
    {
        StartCoroutine(AnimateBrainWave());
    }

    private IEnumerator AnimateBrainWave()
    {
        spriteRenderer.enabled = true;
        for (int i = 0; i < brainWaves.Count; i++)
        {
            spriteRenderer.sprite = brainWaves[i];
            yield return new WaitForSeconds(animationSpeed);
        }
        spriteRenderer.enabled = false;

    }
}
