using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;
using Objects;

namespace UIElements
{
    class Button : CustomObject
    {
        AnimationSprite highlight;
        int releasedFrame;
        int pressedFrame;

        public Button(string filename, int cols, int rows, TiledObject obj) : base(obj, filename, cols, rows) { }

        public override void initialize(Scene parentScene)
        {
            base.initialize(parentScene);
            releasedFrame = currentFrame;
            pressedFrame = releasedFrame + 1;
        }

        public void Update()
        {
            currentFrame = releasedFrame;
            if (HitTestPoint(Input.mouseX, Input.mouseY))
            {
                currentFrame = pressedFrame;
                if (Input.GetMouseButtonDown(0))
                    OnClicked();
            }
        }

        /// <summary>
        /// Should be overridden by objects inheritting this one, gets called when the object gets clicked
        /// </summary>
        protected virtual void OnClicked() { }
    }
}
