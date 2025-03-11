using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionScene : MonoBehaviour
{

    //Will be changed to true seperately as the player triggers something in the scene
    public List<bool> flags;

    public bool CheckDone(){
        foreach(bool flag in flags){
            if(!flag){
                return false;
            }
        }
        return true;
    }

    public void Play(){
        //Actually does nothing
        return;
    }

    public void SetFlag(int index){
        flags[index] = true;
    }
}
