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

        public void OnCollision(GameObject other)
        {
            if (other is Player)
            {
                parentScene.setCheckpoint(this);
            }
        }
    }
}
