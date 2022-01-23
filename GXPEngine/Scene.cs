using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;
using GXPEngine.Core;
using Layers;
using Objects;


class Scene : Pivot
{
    public Player player;
    TiledObject playerObj;
    public Checkpoint currentCheckpoint;
    TiledLoader parser;
    public UI ui;

    //camera movement
    float smoothSpeed = 4f;
    GameObject currentLookTarget;
    
    public Scene(UI ui)
    {
        this.ui = ui;
    }

    void Update()
    {
        //if the scene has something to focus on, update its position and scale to do so
        if (currentLookTarget != null)
        {
            Vector2 globalPosition = currentLookTarget.TransformPoint(0, 0);
            Vector2 relativePosition = this.InverseTransformPoint(globalPosition.x, globalPosition.y);
            Vector2 globalTargetScale = currentLookTarget.InverseTransformDirection(1, 1);
            Vector2 localTargetScale = this.TransformDirection(globalTargetScale.x, globalTargetScale.y);
            float deltaX = (game.width / 2f) - relativePosition.x * this.scaleX - this.x;
            float deltaY = (game.height / 2f) - relativePosition.y * this.scaleY - this.y;
            float deltaScaleX = (-this.scaleX + localTargetScale.x) * smoothSpeed * Time.deltaTime / 1000f;
            float deltaScaleY = (-this.scaleY + localTargetScale.y) * smoothSpeed * Time.deltaTime / 1000f;

            //update position
            Move(deltaX * smoothSpeed * Time.deltaTime / 1000f, deltaY * smoothSpeed * Time.deltaTime / 1000f);

            //update scale
            this.scaleX += deltaScaleX;
            this.scaleY += deltaScaleY;

            //fix position update caused by scale change
            this.x += this.x * deltaScaleX;
            this.y += this.y * deltaScaleY;
        }
    }

    /// <summary>
    /// Will force the camera to teleport to the target, disregarding the smoothing used in Update()
    /// </summary>
    public void jumpToTarget()
    {
        //if the scene has something to focus on, update its position and scale to do so
        if (currentLookTarget != null)
        {
            Vector2 globalPosition = currentLookTarget.TransformPoint(0, 0);
            Vector2 relativePosition = this.InverseTransformPoint(globalPosition.x, globalPosition.y);
            Vector2 globalTargetScale = currentLookTarget.InverseTransformDirection(1, 1);
            Vector2 localTargetScale = this.TransformDirection(globalTargetScale.x, globalTargetScale.y);
            float deltaX = (game.width / 2f) - relativePosition.x * this.scaleX - this.x;
            float deltaY = (game.height / 2f) - relativePosition.y * this.scaleY - this.y;
            float deltaScaleX = -this.scaleX + localTargetScale.x;
            float deltaScaleY = -this.scaleY + localTargetScale.y;
            Move(deltaX, deltaY);

            this.scaleX += deltaScaleX;
            this.scaleY += deltaScaleY;
        }
    }


    /// <summary>
    /// Will initialize the TiledLoader for mapParsing
    /// </summary>
    /// <param name="mapFile">The name of the tiled map to parse/load</param>
    public void loadMapFile(string mapFile)
    {
        parser = new TiledLoader(mapFile, this);
        parser.OnObjectCreated += objectCreateCallback;
    }


    /// <summary>
    /// Will load the level and set up all the different objects for the player to play
    /// </summary>
    public void createLevel()
    {
        //load all image layers with their respective parallax
        parser.addColliders = false;
        int layerCount = 0;
        if (parser.map.ImageLayers != null)
        {
            layerCount = parser.map.ImageLayers.Length;

            for (int i = 0; i < layerCount; i++)
            {
                float parallaxX = parser.map.ImageLayers[i].parallaxx;
                float parallaxY = parser.map.ImageLayers[i].parallaxy;
                Layers.ImageLayer imageLayer = new Layers.ImageLayer(parser, parser.map.ImageLayers[i], this, parallaxX, parallaxY);
                AddChild(imageLayer);
                //parser.rootObject = imageLayer;
                //parser.LoadImageLayers(new int[] { i });
            }
        }

        //instead of loading all tile layers, load each seperately because we dont want collision everywhere
        
        addTileLayer(0, 1 + layerCount, false);
        addTileLayer(1, 2 + layerCount, true);
        addTileLayer(2, 3 + layerCount, false);

        parser.rootObject = this;
        parser.AddManualType(new string[] { "Player" });
        parser.autoInstance = true;
        parser.addColliders = true;
        parser.LoadObjectGroups();

        foreach (GameObject child in  this.GetChildren())
        {
            if(child is CustomObject)
            {
                ((CustomObject)child).initialize(this);
            }
        }

        if (FindObjectOfType<Player>() != null)
            SetChildIndex(FindObjectOfType<Player>(), GetChildCount());//render the player on top of all other gameObjects
        else
            this.SetXY(0, 0);
    }

    /// <summary>
    /// Will add a tile Layer to the game
    /// </summary>
    /// <param name="index">The index of the layer to load</param>
    /// <param name="offset">The z offset of the layer, higher is in front of lower numbers</param>
    /// <param name="generateColliders"> Whether to generate colliders so the player can collide with the tiles</param>
    public void addTileLayer(int index, int offset, bool generateColliders)
    {
        TileLayer newLayer = new TileLayer(this);
        parser.addColliders = generateColliders;
        parser.rootObject = newLayer;
        parser.LoadTileLayers(new int[] { index });
        newLayer.index = index;
        this.SetChildIndex(newLayer, offset);
    }

    public void setLookTarget(GameObject target)
    {
        if(target != null)
            currentLookTarget = target;
    }

    public void objectCreateCallback(Sprite sprite, TiledObject obj)
    {
        if(obj.Type == "Player")
        {
            playerObj = obj;
            player = new Player(obj);
            AddChild(player);
            player.SetXY(obj.X, obj.Y);
            player.initialize(this);
        }
    }

    public void setCheckpoint(Checkpoint point)
    {
        if(currentCheckpoint != point)
        {
            currentCheckpoint = point;
        }
    }

    /// <summary>
    /// Respawns the player at the currentCheckpoint or reloads the level if none available
    /// </summary>
    public void respawn()
    {
        if(currentCheckpoint == null)
        {
            ((MyGame)game).loadNewLevel(((MyGame)game).currentMapName); //reload the level from the game
        }
        else
        {
            player.Destroy();
            player = new Player(playerObj);
            AddChild(player);
            player.SetXY(currentCheckpoint.x, currentCheckpoint.y);
            player.initialize(this);
            player.Move(-currentCheckpoint.width, 0);

            Battery[] batteries = FindObjectsOfType<Battery>();
            foreach (Battery battery in batteries)
            {
                battery.respawn();
            }
        }
    }
}