using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class LeverManager : MonoBehaviour
{
    // The lever manager is a singleton that keep track of the Plate in the list, if all the levers are pulled, the gate will open

    public List<bool> PressurePlateStatus = new List<bool>();
    [SerializeField] private LeverBehaviour lever;

    public void UpdateLeverStatus(int index)
    {
        PressurePlateStatus[index] = true;
        CheckPlateStatus();
    }

    public void CheckPlateStatus()
    {
        Debug.Log("checking pressure plates");
        foreach (bool plate in PressurePlateStatus)
        {
            if (!plate)
            {
                return;
            }
        }
        // All the levers are pulled, open the gate
        ActivateLever();
    }

    private void ActivateLever()
    {
        Debug.Log("Gate Opened");
        lever.EnableLever();
    }

}
