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
        public Button(string filename, int cols, int rows, TiledObject obj) : base(obj, filename, cols, rows)
        {

        }

        public override void initialize(Scene parentScene)
        {
            base.initialize(parentScene);

            //add the highlighted visual
            highlight = new AnimationSprite("Highlight-" + this.texture.filename, this._cols, this._rows, addCollider: false);
            this.AddChild(highlight);
            highlight.currentFrame = this.currentFrame;
            highlight.SetScaleXY(1, 1); //ensure transform is the same
            highlight.SetXY(0, 0);
            highlight.visible = false;
        }

        public void Update()
        {
            if(HitTestPoint(Input.mouseX, Input.mouseY))
                if(Input.GetMouseButtonDown(0))
                    Console.WriteLine("click");
        }
    }
}
