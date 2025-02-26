﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SwacoonNarrative
{
    /// <summary>
    /// Behaviour class for a textbox arrow.
    /// </summary>
    public class SwacoonAdvanceArrow : MonoBehaviour
    {
        [SerializeField] private Image image;

        // Start is called before the first frame update
        void Start()
        {
            SetVisible(false);//start invisible
        }

        /// <summary>
        /// Sets whether er arrow should be visible.
        /// </summary>
        public void SetVisible(bool isVisible)
        {
            image.enabled = isVisible;
        }
    }
}