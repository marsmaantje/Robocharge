using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

namespace Objects
{
    class Sign : CustomObject
    {
        string text;

        public Sign(String filename, int cols, int rows, TiledObject obj) : base(obj, filename, cols, rows, -1, true, true)
        {
            this.collider.isTrigger = true;
        }


        public override void initialize(Scene parentScene)
        {
            base.initialize(parentScene);
            text = obj.GetStringProperty("SignText"); //read the text from the tiledObject
        }

        /// <summary>
        /// Show the text if the player is over the sign
        /// </summary>
        void Update()
        {
            if (HitTest(parentScene.player))
            {
                parentScene.ui.showText(text, 2);
            }
        }
    }
}