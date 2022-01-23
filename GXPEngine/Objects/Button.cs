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

        bool toggle = false;
        bool wasOver = false;

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
                bool colliding = HitTest(parentScene.player);
                if (colliding & !wasOver)
                    isPressed = !isPressed;
                wasOver = colliding;
            }
            else
                isPressed = HitTest(parentScene.player);

            currentFrame = isPressed ? pressedFrame : releasedFrame;
            
        }
    }
}