using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

namespace Objects
{
    class Door : CustomObject
    {
        private const int animationFrames = 7;
        private int startFrame;
        private int endFrame;
        private List<Button> referenceButtons;

        public Door(String fileName, int cols, int rows, TiledObject obj) : base(obj, fileName, cols, rows, -1, true, true)
        {
            referenceButtons = new List<Button>();
        }

        public override void initialize(Scene parentScene)
        {
            base.initialize(parentScene);
            startFrame = currentFrame;
            endFrame = startFrame + animationFrames - 1;

            GameObject[] buttons = parentScene.FindObjectsOfType<Button>();
            foreach (Button button in buttons)
            {
                if (button.obj.Name.Equals(obj.Name))
                {
                    referenceButtons.Add(button);
                }
            }
        }

        void Update()
        {
            //check whether any button is pressed
            bool shouldOpen = false;
            foreach (Button targetButton in referenceButtons)
            {
                if (targetButton.isPressed)
                {
                    shouldOpen = true;
                    break;
                }
            }

            if (shouldOpen)
            {
                if (currentFrame != endFrame)
                {
                    SetCycle(currentFrame, 2);
                }
                else
                {
                    SetCycle(currentFrame, 1);
                }
            }
            else
            {
                if (currentFrame != startFrame)
                {
                    SetCycle(currentFrame - 1, 2);
                }
                else
                {
                    SetCycle(currentFrame, 1);
                }
            }
            Animate(5f * Time.deltaTime / 1000f);

            updateCollision();
        }


        /// <summary>
        /// Checks whether the current frame is equal to the end frame to disable the collider
        /// only enables the collider again if the player is not directly on the door to avoid the player getting stuck
        /// </summary>
        void updateCollision()
        {
            if (currentFrame == endFrame)
                collider.isTrigger = true;
            else if (!collider.HitTest(parentScene.player.collider))
                collider.isTrigger = false;
        }
    }
}
