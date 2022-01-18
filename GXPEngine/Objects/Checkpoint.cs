using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

namespace Objects
{
    class Checkpoint : CustomObject
    {
        public Checkpoint(string filename, int cols, int rows, TiledObject obj) : base(obj, filename, cols, rows) { this.collider.isTrigger = true; }

        /// <summary>
        /// check collision with ONLY the player
        /// </summary>
        public void Update()
        {
            if(HitTest(parentScene.player))
            {
                parentScene.setCheckpoint(this);
                parentScene.player.addEnergy(1);
            }
        }
    }
}
