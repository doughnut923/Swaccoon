using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    private List<BoxBehaviour> boxes = new List<BoxBehaviour>();
    void Awake()
    {
        //get all boats in the scene
        GameObject[] boxObjects = GameObject.FindGameObjectsWithTag("Crate");
        foreach (GameObject boatObject in boxObjects)
        {
            boxes.Add(boatObject.GetComponent<BoxBehaviour>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (BoxBehaviour box in boxes)
        {
            //if the box position is within the water's collider, set the boat IsOnWater to true
            if (Vector2.Distance(transform.position, box.transform.position) < 0.5)
            {
                Debug.Log("Box is in water");
                box.isOnWater = true;
            }
        }
    }
}
