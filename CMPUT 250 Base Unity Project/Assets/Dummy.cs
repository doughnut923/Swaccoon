using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{

    public Transform PassbyGuy;
    public Vector3 TargetPos;

    public void MovePassbuyGuy(){
        PassbyGuy.position = TargetPos;
    }
}
