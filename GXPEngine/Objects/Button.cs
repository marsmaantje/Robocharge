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

        public Button(String fileName, int cols, int rows, TiledObject obj) : base(obj, fileName, cols, rows, -1, true, true)
        {
            this.collider.isTrigger = true;
            this.obj = obj;
        }


        public override void initialize(Scene parentScene)
        {
            base.initialize(parentScene);
            Console.WriteLine(currentFrame);
            releasedFrame = currentFrame;
            pressedFrame = releasedFrame + 1;
        }

        void Update()
        {
            isPressed = HitTest(parentScene.player);
            currentFrame = isPressed ? pressedFrame : releasedFrame;
            
        }
    }
}