using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionScene : MonoBehaviour
{

    //Will be changed to true seperately as the player triggers something in the scene
    public List<bool> flags;

    public UnityEvent OnPlay;

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
        OnPlay.Invoke();
        //set all flags to false
        for(int i = 0; i < flags.Count; i++){
            flags[i] = false;
        }
        return;
    }

    public void SetFlag(int index){
        flags[index] = true;
    }
}
