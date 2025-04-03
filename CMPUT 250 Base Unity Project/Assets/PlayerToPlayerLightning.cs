using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerToPlayerLightning : MonoBehaviour
{

    public static PlayerToPlayerLightning Instance { get; private set; }

    public GameObject lightningGameObject;
    public float SecondsPerFrame = .2f;
    public List<Sprite> lightningSprites = new List<Sprite>(8);
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LightningEffect(Transform from, Transform to)
    {
        Vector3 start = from.position;
        Vector3 end = to.position;
        
        // Calculate the midpoint for the lightning effect
        Vector3 midPoint = (start + end) / 2;
        
        //Set the Position of the lightning game object match the start and end points
        lightningGameObject.transform.position = midPoint;

        //Scale horizontally to match the distance between start and end points
        float distance = Vector3.Distance(start, end);
        lightningGameObject.transform.localScale = new Vector3(1, distance, 1);
        
        // Set the rotation to face the direction of the lightning
        Vector3 direction = (end - start).normalized;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);
        lightningGameObject.transform.rotation = rotation;

        StartCoroutine(CreateLightningEffect(start, end));
    }

    private IEnumerator CreateLightningEffect(Vector3 start, Vector3 end)
    {
        lightningGameObject.SetActive(true);
        
        for (int i = 0; i < lightningSprites.Count; i++)
        {
            lightningGameObject.GetComponent<SpriteRenderer>().sprite = lightningSprites[i];
            yield return new WaitForSeconds(SecondsPerFrame);
        }

        lightningGameObject.SetActive(false);

    }
}
