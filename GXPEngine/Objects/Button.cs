using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

namespace Objects
{
    class Button : CustomObject
    {

        int releasedFrame;
        int pressedFrame;
        public bool isPressed;

        bool toggle = false; //is it a switch or a button
        bool wasOver = false; //was the player over on the previous frame

        public Button(String filename, int cols, int rows, TiledObject obj) : base(obj, filename, cols, rows, -1, true, true)
        {
            this.collider.isTrigger = true;
            this.obj = obj;
        }


        public override void initialize(Scene parentScene)
        {
            base.initialize(parentScene);
            toggle = obj.GetBoolProperty("Toggle", toggle);
            releasedFrame = currentFrame;
            pressedFrame = releasedFrame + 1;
        }

        void Update()
        {
            if (toggle)
            {
                //check if player went over on this frame, toggle the button if so
                bool colliding = HitTest(parentScene.player);
                if (colliding & !wasOver)
                    isPressed = !isPressed;
                wasOver = colliding;
            }
            else
                isPressed = HitTest(parentScene.player);

            currentFrame = isPressed ? pressedFrame : releasedFrame;
            
        }

        /// <summary>
        /// reset the state on respawn
        /// </summary>
        public override void respawn()
        {
            this.toggle = false;
        }
    }
}