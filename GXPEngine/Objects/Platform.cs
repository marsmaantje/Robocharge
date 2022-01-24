using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;
using GXPEngine.Core;

namespace Objects
{
    class Platform : CustomObject
    {
        /// <summary>
        /// Boolean whether the platform stops its movement after it hits something (excluding the player) or after a certain distance
        /// </summary>
        private bool boundByCollision = true;

        /// <summary>
        /// Distance the platform would travel if it is not bound by collision
        /// </summary>
        private float travelDistance = 10;
        private Vector2 startPosition;
        private Vector2 endPosition;

        /// <summary>
        /// whether the platform's direction is flipped by a button or just collision
        /// </summary>
        private bool buttonActivated = false;

        /// <summary>
        /// direction (in degrees) the platform will move in when no buttons are pressed and the level starts playing
        /// </summary>
        private int startDirection = 0;
        private Vector2 startDirectionVector;
        private Vector2 reverseDirectionVector;

        /// <summary>
        /// time the platform will remain stationary after hitting a wall before moving the other way
        /// </summary>
        private float pauseTime = 0;

        /// <summary>
        /// speed of the platform in pixels per second
        /// </summary>
        private float speed = 16;

        /// <summary>
        /// List of buttons that will trigger the platform
        /// </summary>
        private List<Button> referenceButtons;

        /// <summary>
        /// Indicator whether the platform is going in the startDirection or the reverse
        /// </summary>
        private bool isReversed = false;

        private int waitStart = 0;
        private bool isWaiting = false;

        private Sprite playerParenter;

        public Platform(String filename, int cols, int rows, TiledObject obj) : base(obj, filename, cols, rows)
        {
            referenceButtons = new List<Button>();
            readVariables();
        }

        private void readVariables()
        {
            boundByCollision = obj.GetBoolProperty("boundByCollision", boundByCollision);


            buttonActivated = obj.GetBoolProperty("buttonActivated", buttonActivated);
            startDirection = obj.GetIntProperty("startDirection", startDirection);

            pauseTime = obj.GetFloatProperty("pauseTime", pauseTime);
            speed = obj.GetFloatProperty("speed", speed);
        }


        public override void initialize(Scene parentScene)
        {
            base.initialize(parentScene);

            //if the platform is button activated, generate a list of all buttons that can activate the platform
            if (buttonActivated)
            {
                GameObject[] buttons = parentScene.FindObjectsOfType<Button>();
                foreach (Button button in buttons)
                {
                    if (button.obj.Name.Equals(obj.Name))
                    {
                        referenceButtons.Add(button);
                    }
                }

                if (referenceButtons.Count == 0)
                {
                    throw (new Exception("No Buttons found for " + obj.Name));
                }
            }

            //calculate the startDirection and reverseDirection
            float startAngle = (Mathf.PI / 180f) * (startDirection); //convert angle from degrees to radians
            startDirectionVector.x = Mathf.Sin(startAngle);
            startDirectionVector.y = Mathf.Cos(startAngle);

            reverseDirectionVector.x = -startDirectionVector.x;
            reverseDirectionVector.y = -startDirectionVector.y;

            startPosition = new Vector2(this.x, this.y);

            //initialize the start and endPoints if not bound by collision
            if (!boundByCollision)
            {
                endPosition = new Vector2(this.x + (startDirectionVector.x * travelDistance * 16), this.y + (startDirectionVector.y * travelDistance * 16)); // * 16 because one tile is 16 pixels
            }

            //initialize the playerParenter
            playerParenter = new Sprite("sprites/empty.png");
            this.AddChild(playerParenter);
            playerParenter.collider.isTrigger = true;
            playerParenter.width = 14;
            playerParenter.height = 8;
            playerParenter.SetOrigin(7, 8); //set origin at middle-bottom
            playerParenter.SetXY(-1, -7); //move so it is sticking out of the top
            playerParenter.visible = false;
        }

        public void Update()
        {
            if (buttonActivated)
            {
                //check if any button is pressed
                bool buttonPressed = false;
                foreach (Button button in referenceButtons)
                {
                    if (button.isPressed)
                    {
                        buttonPressed = true;
                        break;
                    }
                }

                //TODO: implement button pressing
                MovePlatform(buttonPressed ? reverseDirectionVector : startDirectionVector, speed);
            }
            else
            {
                if (isWaiting)
                {
                    if ((Time.now - waitStart) / 1000f > pauseTime)
                    {
                        isWaiting = false;
                        isReversed = !isReversed;
                    }
                }
                else
                {
                    if (!MovePlatform(isReversed ? reverseDirectionVector : startDirectionVector, speed))
                    {
                        isWaiting = true;
                        waitStart = Time.now;
                    }
                }
            }
        }

        /// <summary>
        /// Tries to move the platform with the given vector at the given speed for one frame
        /// </summary>
        /// <param name="direction"> the direction ot move the platform in</param>
        /// <param name="movementSpeed">the speed to move the platform at</param>
        /// <returns>boolean whether it moved succesfully, will still return true when blocked by player</returns>
        private bool MovePlatform(Vector2 direction, float movementSpeed)
        {
            direction.normalize();
            float vx = direction.x * movementSpeed * Time.deltaTime / 1000f;
            float vy = direction.y * movementSpeed * Time.deltaTime / 1000f;

            Collision collision;
            bool isPlayerOn = playerParenter.HitTest(parentScene.player);
            if (isPlayerOn)
            {
                parentScene.player.Move(1000, 0);//temporarily move the player far away
                collision = MoveUntilCollision(vx, vy);
                parentScene.player.Move(-1000, 0);
                //only move the player if the platform did not collide with anything,
                //otherwise the player continues moving even though the platform has stopped
                if(collision == null)
                    parentScene.player.Move(vx, vy);

            }
            else
            {
                collision = MoveUntilCollision(vx, vy);
            }

            return (collision == null || collision.other is Player);
        }

        public override void respawn()
        {
            this.x = startPosition.x;
            this.y = startPosition.y;
            this.isReversed = false;
        }
    }
}
