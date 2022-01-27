using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

namespace Objects
{
    class DeathTrigger : CustomObject
    {

        public DeathTrigger(string filename, int cols, int rows, TiledObject obj) : base(obj, filename, cols, rows)
        {
            collider.isTrigger = true;
            this.visible = obj.GetBoolProperty("isVisible", true);
        }

        public override void initialize(Scene parentScene)
        {
            base.initialize(parentScene);
        }

        /// <summary>
        /// Kill the player if he is colliding
        /// </summary>
        /// <param name="other">other object it collided with</param>
        public void OnCollision(GameObject other)
        {
            if(other is Player)
            {
                ((Player)other).die();
            }
        }
    }
}
