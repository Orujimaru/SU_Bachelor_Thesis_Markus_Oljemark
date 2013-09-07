using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Orujin.Core.Renderer;

namespace Orujin.Core.Input
{
    /***Extend to support more than just left and right button***/
    public class MouseManager
    {
        public static bool leftButtonDown {get; private set;}
        public static bool rightButtonDown {get; private set;}

        public static bool leftButtonReleased { get; private set; }
        public static bool rightButtonReleased { get; private set; }

        public static bool leftButtonClicked { get; private set; }
        public static bool rightButtonClicked { get; private set; }

        public static float leftButtonCurrentStateElapsed { get; private set; }
        public static float rightButtonCurrentStateElapsed { get; private set; }

        public static Vector2 mousePosition { get; private set; }

        private MouseState mouseState;

        public MouseManager()
        {
            leftButtonDown = false;
            rightButtonDown = false;
            leftButtonCurrentStateElapsed = 0;
            rightButtonCurrentStateElapsed = 0;
            mousePosition = new Vector2(0,0);
        }

        public void Update(float elapsedTime)
        {
            leftButtonClicked = false;
            rightButtonClicked = false;
            mouseState = Mouse.GetState();

            //Handle left button
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!leftButtonDown)
                {
                    leftButtonClicked = true;
                    leftButtonDown = true;
                    leftButtonCurrentStateElapsed = 0;
                }
                leftButtonCurrentStateElapsed += elapsedTime;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                if (leftButtonDown)
                {
                    leftButtonDown = false;
                    leftButtonReleased = true;
                    leftButtonCurrentStateElapsed = 0;
                }
                else
                {
                    if (leftButtonReleased)
                    {
                        leftButtonReleased = false;
                    }
                    leftButtonCurrentStateElapsed += elapsedTime;
                }
            }

            //Handle right button
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!rightButtonDown)
                {
                    rightButtonClicked = true;
                    rightButtonDown = true;
                    rightButtonCurrentStateElapsed = 0;
                }
                rightButtonCurrentStateElapsed += elapsedTime;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                if (rightButtonDown)
                {
                    rightButtonDown = false;
                    rightButtonReleased = true;
                    rightButtonCurrentStateElapsed = 0;
                }
                else
                {
                    if (rightButtonReleased)
                    {
                        rightButtonReleased = false;
                    }
                    rightButtonCurrentStateElapsed += elapsedTime;
                }
            }

            mousePosition = Vector2.Transform(new Vector2(mouseState.X, mouseState.Y),
                                    Matrix.Invert(Camera.matrix));
        }
    }
}
