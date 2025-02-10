using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{

    private Vector3 TargetPosition;
    private Vector3 StartPosition;
    
    [SerializeField] AnimationCurve riseCurve;
    [SerializeField] float riseTime = 1f;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] private AudioSource platformSoundSource;
    [SerializeField] private AudioClip platformRiseSound;
    private float minPitch = 0.5f;
    private float maxPitch = 1f;

    void Awake(){
        TargetPosition = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y-10, transform.position.z);
        StartPosition = transform.position;
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }
    public void Rise()
    {
        CameraManager.Instance.ShakeCamera(riseTime, .1f);
        StartCoroutine(RisePlatform());
    }

    IEnumerator RisePlatform()
    {
        //use the curve to move the platform up also change the opcaity of the platform
        float time = 0;
        while (time < riseTime)
        {
            platformSoundSource.clip = platformRiseSound;
            platformSoundSource.volume = 0.5f;

            time += Time.deltaTime;
            float t = time / riseTime;

            platformSoundSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
            platformSoundSource.Play();
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, riseCurve.Evaluate(t));
            spriteRenderer.color = new Color(1, 1, 1, riseCurve.Evaluate(t));
            yield return null;
        }
    }
}
