using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using TiledMapParser;
using Objects;

class Player : AnimationSprite
{
    //tiled object to read data from
    TiledObject obj;

    //movement speed
    private float movementSpeed = 50;
    private float jumpSpeed = 4f;
    private bool grounded = false;
    private float xSpeed, ySpeed = 0;
    private float drag = 0.8f;
    private float stoppedMargin = 0.1f;
    private float gravity = 9.81f;

    private Scene parentScene;

    //camera target when following the player
    public Pivot cameraTarget;

    public Player() : base("character_robot_sheet.png", 9, 5, -1, true, true)
    {
        //set the animation to its idle state and add the speedIndicator
        currentFrame = 0;
        SetCycle(0, 1);

        //setup the camera pivot
        cameraTarget = new Pivot();
        AddChild(cameraTarget);
        cameraTarget.SetXY(0, -height * 2);
    }

    public Player(String filename, int cols, int rows, TiledObject obj) : base("character_robot_sheet.png", 9, 5)
    {
        this.obj = obj;

        //set the animation to its idle state and add the speedIndicator
        currentFrame = 0;
        SetCycle(0, 1);

        //setup the camera pivot
        cameraTarget = new Pivot();
        AddChild(cameraTarget);
        cameraTarget.SetXY(width / 2, -height);
    }

    public void initialize(Scene parentScene)
    {
        this.parentScene = parentScene;
        this.SetOrigin(width / 2, height);
    }

    void Update()
    {
        //playerMovement();
        playerAnimation();
        updateCameraTarget();
    }

    /// <summary>
    /// checks whether the player is colliding with a CameraTrigger, if this is the case it will set the cameraTarget of the scene to that instead of the player
    /// </summary>
    private void updateCameraTarget()
    {
        GameObject[] collisions = this.GetCollisions(true, false);
        foreach (GameObject collision in collisions)
        {
            if (collision is CameraTrigger)
            {
                parentScene.setLookTarget(collision);
            }
        }
        parentScene.setLookTarget(this.cameraTarget);
        cameraTarget.SetXY(0, -height * this.scaleY);
        cameraTarget.SetScaleXY(4, 4);

    }

    /// <summary>
    /// moves the player and updates all the visuals accordingly
    /// </summary>
    private void playerAnimation()
    {

        //update the XSpeed based on A and/or D pressed and make this framerate independent
        xSpeed = ((Input.GetKey(Key.D) ? 1 : 0) - (Input.GetKey(Key.A) ? 1 : 0)) * movementSpeed * Time.deltaTime / 1000.0f;

        //jumping with gravity
        ySpeed += gravity * Time.deltaTime / 1000f;

        //jump when W or SPACE is pressed and the character is on the ground
        if ((Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.SPACE)) && grounded)
            ySpeed = -jumpSpeed;

        Collision collision;
        collision = MoveUntilCollision(0, ySpeed);
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

        //animate the character at x fps
        Animate(8f * Time.deltaTime / 1000f);

        //set the mirror and cycle based on direction of movement
        if (xSpeed != 0)
            Mirror(xSpeed < 0, false);

        if (ySpeed < -stoppedMargin) //falling down
            SetCycle(1, 1);
        else if (ySpeed > stoppedMargin) //jumpping up
            SetCycle(2, 1);
        else if (xSpeed > stoppedMargin || xSpeed < -stoppedMargin)
        {
            if (Input.GetKey(Key.LEFT_SHIFT)) //running
                SetCycle(24, 3);
            else
                SetCycle(36, 8); //walking
        }
        else
            SetCycle(0, 1); //idle
    }

    /// <summary>
    /// updates the x and y speed according to pressed keys and player state
    /// </summary>
    private void playerMovement()
    {
        //update the XSpeed based on A and/or D pressed and make this framerate independent
        xSpeed = ((Input.GetKey(Key.D) ? 1 : 0) - (Input.GetKey(Key.A) ? 1 : 0)) * movementSpeed * Time.deltaTime / 1000.0f;

        //jumping with gravity
        ySpeed += gravity * Time.deltaTime / 1000f;

        //jump when W or SPACE is pressed and the character is on the ground
        if ((Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.SPACE)) && grounded)
            ySpeed = -jumpSpeed;

        //xSpeed += ((Input.GetKeyDown(Key.D) ? 1 : 0) - (Input.GetKeyDown(Key.A) ? 1 : 0)) * jumpSpeed;
        //xSpeed *= drag;
    }
}
