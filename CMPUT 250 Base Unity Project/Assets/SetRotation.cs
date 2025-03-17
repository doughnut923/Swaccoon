using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotation : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    public Vector3 rot = Vector3.up;
    public Transform target;

    public void SetRot(){
        target.rotation = Quaternion.Euler(rot);
    }
}
