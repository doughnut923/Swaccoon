using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlateBehviour : MonoBehaviour
{
    // The Pressure Plate Behavious controls the behaviour of the attached pressure plate and does the following:
    // 1. Check if the player is on the pressure plate
    // 2. If the player is a horse i.e. have HorseBehaviour, then the pressure plate will be activated
    // 3. If the player is not a horse, the pressure plate will not be activated
    // 4. If the pressure plate is activated, it will invoke the onPlatePressed event

    [SerializeField] private bool isPressed = false;
    public bool IsPressed { get { return isPressed; } }
    public UnityEvent onPlatePressed = new UnityEvent();
    public UnityEvent onPlateReleased = new UnityEvent();

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Pressure Plate Triggered");
        //Debug.Log(collision.gameObject.name);
        //Debug.Log(collision.gameObject.GetComponent<HorseBehaviour>());
        if (collision.gameObject.GetComponent<HorseBehaviour>() != null)
        {
            isPressed = true;
            onPlatePressed.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<HorseBehaviour>())
        {
            isPressed = false;
            onPlateReleased.Invoke();
        }
    }
}
