using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

namespace UIElements
{
    class EnergyBar : AnimationSprite
    {
        EasyDraw healthMeter;
        float[] energyParam = { 0, 0 };

        /// <summary>
        /// Creates a new EnergyBar
        /// </summary>
        /// <param name="currentEnergy">Amount of energy you want to start with</param>
        /// <param name="maxEnergy">Maximum energy the bar will display</param>
        public EnergyBar(float currentEnergy, float maxEnergy) : base("sprites/energyBar.png", 1, 3, addCollider: false)
        {
            this.energy = currentEnergy;
            this.maxEnergy = maxEnergy;

            //add the actual meter at specific position according to the texture
            healthMeter = new EasyDraw("sprites/white.png", false);
            AddChild(healthMeter);
            healthMeter.width = 100;
            healthMeter.height = 8;
            healthMeter.SetXY(-40, 2);
            healthMeter.color = 0x00ff00;
        }

        /// <summary>
        /// get or set the energy, will automatically update the visual if changed
        /// </summary>
        public float energy
        {
            get
            {
                return energyParam[1];
            }

            set
            {
                energyParam[1] = value;
                updateVisuals();
            }
        }

        /// <summary>
        /// get or set the maximum energy, will automatically update the visual if changed
        /// </summary>
        public float maxEnergy
        { 
            get
            {
                return energyParam[0];
            }
            set
            {
                energyParam[0] = value;
                updateVisuals();
            }
        }

        /// <summary>
        /// Update the visual of the energy to represent its change
        /// </summary>
        private void updateVisuals()
        {
            byte R = (byte)(255 - (Mathf.Pow(energy / maxEnergy, 4f) * 255));
            byte G = (byte)(Mathf.Pow(energy / maxEnergy, 0.25f) * 255);
            byte B = 0;
            uint hex = (uint)((255 << 24) | ((byte)R << 16) | ((byte)G << 8) | ((Byte)B << 0));
            if (healthMeter != null)
            {
                healthMeter.color = hex;
                healthMeter.width = (int)(energy / maxEnergy * 100);
            }
            currentFrame = 2 - (int)(energy / (maxEnergy / 3));
        }


    }
}
