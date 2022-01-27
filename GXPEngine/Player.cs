using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using TiledMapParser;
using Objects;
using UIElements;

class Player : EasyDraw
{
    private const int objectWidth = 14;
    private const int objectHeight = 22;

    //tiled object to read data from
    TiledObject obj;

    //movement speed
    private float walkSpeed = 50f;
    private float runSpeed = 80f;
    private float movementSpeed = 50;
    private float jumpSpeed = 170f;
    private bool grounded = false;
    private float xSpeed, ySpeed = 0;
    private float stoppedMargin = 0.1f;
    private float gravity = 300;

    private Scene parentScene;
    private AnimationSprite animation;

    //camera target when following the player
    public Pivot cameraTarget;
    private float cameraSize = 6f;

    //sounds
    Sound jumpSound = new Sound("sounds/jump.wav");
    Sound stepSound = new Sound("sounds/step.wav");
    Sound deathSound = new Sound("sounds/explosion.wav");
    List<int> stepFrames = new List<int>() { 6, 9 };

    private float maxEnergy = 100; // maximum energy you can have
    private float energy = 100; //amount of energy you start with
    private float walkEnergyCost = 0.1f; //cost per movementSpeed per second, e.g. a movement speed of 50 results in 0.1 * 50 = 5 energy lost per second
    private float jumpEnergyCost = 10f; //cost per jump, gets subtracted the moment you jump
    private float idleEnergyCost = 1f; //static energy drain that always gets applied
    private bool isDying = false; //flag whether the death animation is playing, caused by e.g. spikes
    private bool isDead = false; //flag whether the player is dead, either due to the dying or energy running out

    EnergyBar energyBar;


    public Player() : base(objectWidth, objectHeight, true) { readVariables(); }

    public Player(TiledObject obj) : base(objectWidth, objectHeight, true)
    {
        this.obj = obj;
        readVariables();
    }

    public Player(String filename, int cols, int rows, TiledObject obj) : base(objectWidth, objectHeight, true)
    {
        this.obj = obj;
        readVariables();
    }

    /// <summary>
    /// setup the visual for the player,
    /// visual is seperate for collision reasons
    /// </summary>
    private void createAnimation()
    {
        animation = new AnimationSprite("sprites/OrangeRobot_SpriteSheet.png", 8, 5, -1, true, false);
        AddChild(animation);
        animation.SetOrigin(animation.width / 2, animation.height);
        animation.SetXY(0, 0);

        //set the animation to its idle state and add the speedIndicator
        animation.currentFrame = 0;
        animation.SetCycle(0, 5);
    }

    /// <summary>
    /// Initializes the sprite for the player and the cameraTarget
    /// </summary>
    /// <param name="parentScene">The scene this player should callback for change of camera target</param>
    public void initialize(Scene parentScene)
    {
        this.parentScene = parentScene;
        this.SetOrigin(width / 2, height);
        createAnimation();
        this.x += animation.width / 2; //offset for the tiled player position/origin



        //setup the camera pivot
        cameraTarget = new Pivot();
        AddChild(cameraTarget);
        float targetX = (float)this.width / (float)game.width;
        float targetY = (float)this.height / (float)game.height;
        float targetSize = Mathf.Max(targetX, targetY); //make sure the camera scale is uniform
        Vector2 actualSize = InverseTransformDirection(targetSize, targetSize);
        actualSize *= new Vector2(parentScene.scaleX, parentScene.scaleY);
        cameraTarget.SetXY(0, -height * 1.3f);
        cameraTarget.SetScaleXY(actualSize.x * cameraSize, actualSize.y * cameraSize); //MATH!
        parentScene.setLookTarget(cameraTarget);
        parentScene.jumpToTarget();


        //setup the energyBar, if it exists, delete the existing one
        parentScene.ui.removeElement("energyBar");
        energyBar = new EnergyBar(energy, maxEnergy);
        energyBar.SetOrigin(energyBar.width / 2, 0);
        parentScene.ui.addElement(energyBar, "energyBar", game.width / 2, 0);
    }

    /// <summary>
    /// Try to read the player variables from the tiled object, use the fallback variables if not present
    /// </summary>
    private void readVariables()
    {
        walkSpeed = obj.GetFloatProperty("walkSpeed", walkSpeed);
        runSpeed = obj.GetFloatProperty("runSpeed", runSpeed);
        jumpSpeed = obj.GetFloatProperty("jumpSpeed", jumpSpeed);
        gravity = obj.GetFloatProperty("gravity", gravity);
        maxEnergy = obj.GetFloatProperty("maxEnergy", maxEnergy);
        energy = obj.GetFloatProperty("energy", energy);
        walkEnergyCost = obj.GetFloatProperty("walkEnergyCost", walkEnergyCost);
        jumpEnergyCost = obj.GetFloatProperty("jumpEnergyCost", jumpEnergyCost);
        idleEnergyCost = obj.GetFloatProperty("idleEnergyCost", idleEnergyCost);

    }

    void Update()
    {
        playerAnimation();
        updateCameraTarget();
        updateUI();

        if (isDead && Input.GetKeyDown(Key.E))
            parentScene.respawn();//((MyGame)game).loadNewLevel(((MyGame)game).currentMapName);
    }

    /// <summary>
    /// checks whether the player is colliding with a CameraTrigger, if this is the case it will set the cameraTarget of the scene to that instead of the player
    /// </summary>
    private void updateCameraTarget()
    {
        GameObject[] collisions = this.GetCollisions(true, false);
        foreach (GameObject collision in collisions)
        {
            if (collision is Objects.CameraTrigger)
            {
                parentScene.setLookTarget(((CameraTrigger)collision).target);
                return;
            }
        }
        parentScene.setLookTarget(this.cameraTarget);

    }

    /// <summary>
    /// moves the player and updates all the visuals accordingly
    /// </summary>
    private void playerAnimation()
    {
        if(!isDying) //only move the player if he is not dying
            movePlayer();

        int prevFrame = animation.currentFrame;
        //animate the character at x fps
        if (energy > 0)
        {
            animation.Animate(8f * Time.deltaTime / 1000f);
            if (!isDying)
                isDead = false;
        }
        else
            isDead = true; //if I am out of energy, I am dead

        //play the step sound if the animation switches to a frame where the player is stepping
        if (prevFrame != animation.currentFrame && stepFrames.Contains(animation.currentFrame))
        {
            stepSound.Play().Frequency = (Mathf.Sin(this.x + Time.now / 100f) * 10000 + 44100); //play at a random frequency to add variation
        }
        
        //set the mirror and cycle based on direction of movement
        if (xSpeed != 0)
            animation.Mirror(xSpeed < 0, false);

        if (!isDying) //only update the animation frames if the player isnt currently dying
        {
            if (ySpeed < -stoppedMargin) //falling down
                animation.SetCycle(11, 1);
            else if (ySpeed > stoppedMargin) //jumpping up
                animation.SetCycle(15, 1);
            else if (xSpeed > stoppedMargin || xSpeed < -stoppedMargin)
            {
                animation.SetCycle(5, 6); //walking
                if (Input.GetKey(Key.LEFT_SHIFT)) //if running, increase the framerate
                    animation.Animate(4f * Time.deltaTime / 1000f);
            }
            else
                animation.SetCycle(0, 5); //idle
        }
        else
        {
            //if the player is dying, check whether the animation is finished and stay on the last frame if so
            if(animation.currentFrame != 33)
            {
                animation.SetCycle(29, 5);
            }
            else
            {
                animation.SetCycle(33, 1);
                isDead = true;
            }
        }
    }

    private void movePlayer()
    {
        bool hasEnergy = energy > 0;

        movementSpeed = (Input.GetKey(Key.LEFT_SHIFT) ? runSpeed : walkSpeed);
        //update the XSpeed based on A and/or D pressed and make this framerate independent
        //only register input if we have energy
        xSpeed = ((Input.GetKey(Key.D) && hasEnergy ? 1 : 0) - (Input.GetKey(Key.A) && hasEnergy ? 1 : 0)) * movementSpeed * Mathf.Min(Time.deltaTime / 1000f, 0.03f);

        if (isOverLadder())
            ySpeed = ((Input.GetKey(Key.S) && hasEnergy ? 1 : 0) - (Input.GetKey(Key.W) && hasEnergy ? 1 : 0)) * movementSpeed;
        else
        {
            //jumping with gravity
            ySpeed += gravity * Time.deltaTime / 1000f;

            //jump when W or SPACE is pressed and the character is on the ground
            if ((Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.SPACE)) && grounded && energy > 0)
            {
                ySpeed = -jumpSpeed;
                jumpSound.Play().Frequency = (Mathf.Sin(this.x + Time.now / 100f) * 4000 + 44100); //play the jump sound with a random pitch
                energy -= jumpEnergyCost; //jump energy decrease
            }
        }

        //move the player
        Collision collision;
        collision = MoveUntilCollision(0, ySpeed * Mathf.Min(Time.deltaTime / 1000f, 0.03f));
        if (collision != null)
        {
            if (ySpeed > 0)
                grounded = true;
            ySpeed = 0;
        }
        else
            grounded = false;

        collision = MoveUntilCollision(xSpeed, 0);
        if (collision != null)
        {
            xSpeed = 0;
            x = (int)x;
        }
        else //if we moved succesfully, subtract energy
        {
            energy -= Mathf.Abs(xSpeed * walkEnergyCost);
        }

        //static energy decrease
        energy -= idleEnergyCost * Time.deltaTime / 1000f;

    }

    /// <summary>
    /// Updates the Energy bar at the top of the screen
    /// </summary>
    private void updateUI()
    {
        energy = Mathf.Max(energy, 0);
        energyBar.energy = energy;
        if(isDead)
        {
            parentScene.ui.showText("[E] to respawn", 100);
        }
    }

    /// <summary>
    /// Checks whether the player is over a ladder for changing the locomotion
    /// </summary>
    /// <returns>Whether the player is currently on a ladder</returns>
    private bool isOverLadder()
    {
        GameObject[] collisions = this.GetCollisions(true, false);
        foreach (GameObject collision in collisions)
        {
            if (collision is Objects.Ladder)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Adds the specified amount of energy to the player and clamps it to the max energy
    /// </summary>
    /// <param name="amount">amount of energy to add</param>
    public void addEnergy(float amount)
    {
        energy += amount;
        energy = Mathf.Min(energy, maxEnergy);
    }

    /// <summary>
    /// initiates the explosion of the player death
    /// </summary>
    public void die()
    {
        if(!isDying) //ensure the sound only plays once
            deathSound.Play().Frequency = (Mathf.Sin(this.x + Time.now / 100f) * 10000 + 44100);
        isDying = true;
    }
}