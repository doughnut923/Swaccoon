using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    //Create Instance
    public static PlatformManager Instance {get; private set;}
    [SerializeField] public List<PlatformBehaviour> platforms = new List<PlatformBehaviour>();

    void Awake(){
        if(Instance == null){
            Instance = this;
        }else{
            Destroy(gameObject);
        }
    }
}
