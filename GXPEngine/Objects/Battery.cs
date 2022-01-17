using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

namespace Objects
{
    class Battery : CustomObject
    {
        float energy = 0;

        public Battery(string fileName, int cols, int rows, TiledObject obj) : base(obj, fileName, cols, rows)
        {
            this.collider.isTrigger = true;
            energy = obj.GetFloatProperty("energy", energy);
        }

        public override void initialize(Scene parentScene)
        {
            base.initialize(parentScene);
        }
        
        

        /// <summary>
        ///check if we are colliding with the player, and ONLY the player
        /// </summary>
        public void Update()
        {
            if(HitTest(parentScene.player))
            {
                //add energy to the player
                parentScene.player.addEnergy(energy);
                this.LateDestroy();
            }
        }
    }
}
