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
        public Checkpoint(string fileName, int cols, int rows, TiledObject obj) : base(obj, fileName, cols, rows) { this.collider.isTrigger = true; }

        public void OnCollision(GameObject other)
        {
            if (other is Player)
            {
                parentScene.setCheckpoint(this);
            }
        }
    }
}
