using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SwacoonNarrative
{
    /// <summary>
    /// A component that recieves clicks and broadcasts a callback.
    /// Can set reciever methods in the editor.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SwacoonClickableObject : MonoBehaviour
    {
        //Event Callbacks
        public UnityEvent onClick;


        /// <summary>
        /// Called when this object is clicked on
        /// </summary>
        private void OnMouseDown()
        {
            onClick.Invoke();
        }
    }
}