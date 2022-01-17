using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;
using GXPEngine.Core;

namespace Objects
{
    class Ladder : CustomObject
    {
        public Ladder(String fileName, int cols, int rows, TiledObject obj) : base(obj, fileName, cols, rows, -1, true, true)
        {
            this.collider.isTrigger = true;
        }
    }
}