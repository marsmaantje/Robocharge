using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

namespace Objects
{
    class CustomObject : AnimationSprite
    {
        public TiledObject obj;
        protected Scene parentScene;

        public CustomObject(TiledObject obj, String fileName, int cols, int rows, int frames = -1, bool keepInCache = false, bool addCollider = true) : base(fileName, cols, rows, frames, keepInCache, addCollider)
        {
            this.obj = obj;
        }

        public virtual void initialize(Scene parentScene)
        {
            this.parentScene = parentScene;
        }
    }
}