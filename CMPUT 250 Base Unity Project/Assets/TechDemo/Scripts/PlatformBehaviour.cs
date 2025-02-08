using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{

    private Vector3 TargetPosition;
    [Range(0.01f, 1f)]
    [SerializeField] private float riseSpeed = 0.1f;

    void Awake(){
        TargetPosition = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y-100, transform.position.z);
    }
    public void Rise()
    {
        StartCoroutine(RisePlatform());
    }

    IEnumerator RisePlatform()
    {
        while (transform.position.y < 0)
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, riseSpeed);
            yield return new WaitForFixedUpdate();
        }
    }
}
