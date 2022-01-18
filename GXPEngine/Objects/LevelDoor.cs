using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

namespace Objects
{
    class LevelDoor : CustomObject
    {
        string text;
        string mapName;

        public LevelDoor(String filename, int cols, int rows, TiledObject obj) : base(obj, filename, cols, rows, -1, true, true)
        {
            this.collider.isTrigger = true;
        }


        public override void initialize(Scene parentScene)
        {
            base.initialize(parentScene);
            text = obj.GetStringProperty("SignText");
            mapName = obj.GetStringProperty("MapName", ((MyGame)game).currentMapName);
        }

        void Update()
        {
            if (HitTest(parentScene.player))
            {
                parentScene.ui.showText(text, 2);
                if(Input.GetKeyDown(Key.E))
                {
                    ((MyGame)game).loadNewLevel(mapName);
                }
            }
        }
    }
}