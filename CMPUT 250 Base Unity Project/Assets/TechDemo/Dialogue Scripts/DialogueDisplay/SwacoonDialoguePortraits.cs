﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwacoonNarrative
{
    /// <summary>
    /// Recieves commands from the dialogue sequencer to control portraits.
    /// </summary>
    public class SwacoonDialoguePortraits : MonoBehaviour
    {

        [SerializeField] SwacoonDialoguePortrait portraitLeft, portraitRight;



        /// <summary>
        /// Sets the sprite of the left portrait.
        /// If the portrait isn't showing it will fade in.
        /// </summary>
        /// <param name="sprite">Sprite to set the portrait to</param>
        public void SetPortraitSpriteLeft(Sprite sprite)
        {
            portraitLeft.SetSprite(sprite);

            //Fade if not already there
            if (!portraitLeft.IsEntered)
            {
                portraitLeft.EnterFade(0.5f);
            }
        }

         /// <summary>
        /// Sets the sprite of the right portrait.
        /// If the portrait isn't showing it will fade in.
        /// </summary>
        /// <param name="sprite">Sprite to set the portrait to</param>
        public void SetPortraitSpriteRight(Sprite sprite)
        {
            portraitRight.SetSprite(sprite);

            //Fade if not already there
            if (!portraitRight.IsEntered)
            {
                portraitRight.EnterFade(0.5f);
            }
        }

        /// <summary>
        /// Fades out left portrait.
        /// </summary>
        public void ClosePortraitLeft()
        {
            if (portraitLeft.IsVisible && portraitLeft.IsEntered == true)
            {
                portraitLeft.ExitFade(0.5f);
            }
        }

        /// <summary>
        /// Fades out right portrait.
        /// </summary>
        public void ClosePortraitRight()
        {
            if (portraitRight.IsVisible && portraitRight.IsEntered == true)
            {
                portraitRight.ExitFade(0.5f);
            }
        }

        /// <summary>
        /// Fades out all portraits.
        /// </summary>
        public void ClosePortraits()
        {
            ClosePortraitLeft();
            ClosePortraitRight();
        }
    }
}