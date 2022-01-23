using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using System.IO;

namespace Layers
{
    class ImageLayer : Layer
    {

        public ImageLayer(TiledMapParser.TiledLoader loader, TiledMapParser.ImageLayer obj, Scene parentScene, float paralaxX = 1, float paralaxY = 1) : base(parentScene, paralaxX, paralaxY)
        {
            bool repeating = obj.GetBoolProperty("Repeating", false);

            for (int i = 0; i < (repeating ? 10 : 1); i++)
            {
                Sprite image = new Sprite(Path.Combine(loader._foldername, obj.Image.FileName), false, false);
                image.x = obj.offsetX + (image.width * i);
                image.y = obj.offsetY;
                image.alpha = obj.Opacity;

                AddChild(image);
            }   

        }
    }
}
