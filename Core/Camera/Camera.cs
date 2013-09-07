using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Orujin.Framework;

namespace Orujin.Core.Renderer
{
    public class Camera
    {
        public static GameObject parent { get; private set; }
        public static bool followParentHorizontally { get; internal set; }
        public static bool followParentVertically { get; internal set; }

        public static Nullable<Vector2> destination { get; internal set; }
        public static float duration { get; internal set; }

        public static Nullable<Vector2> scaleDestination { get; internal set; }
        public static float scaleDuration { get; internal set; }

        public static Nullable<float> rotationDestination { get; internal set; }
        public static float rotationDuration { get; internal set; }

        public const float PixelsPerMeter = 64.0f;

        private static Vector2 position;
        public static Vector2 adjustedPosition
        {
            get
            {
                return -position;
            }
            private set
            {
                return;
            }
        }

        public static Vector2 screenCenter { get; internal set; }
        public static Vector2 scale { get; internal set; }
        public static float rotation { get; internal set; }

        public static Matrix matrix { 
            get 
            {
                return Matrix.CreateTranslation(new Vector3(position - screenCenter, 0f))
                    * Matrix.CreateRotationZ(rotation)
                    * Matrix.CreateScale(new Vector3(scale, 1))
                    * Matrix.CreateTranslation(new Vector3(screenCenter, 0f));
            }
            private set { return; }
        }

        public static Matrix farseerMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3((position - screenCenter) / PixelsPerMeter, 0f))
                    * Matrix.CreateScale(new Vector3(scale, 1))
                    * Matrix.CreateTranslation(new Vector3(screenCenter / PixelsPerMeter, 0f));
            }
            private set { return; }
        }

        public static float activeArea = 1f;
        public static Rectangle activeBounds { get; private set; }
        public static Rectangle matrixBounds { get; private set; }

        internal static void Initialize(float frameWidth, float frameHeight)
        {
            screenCenter = new Vector2(frameWidth / 2, frameHeight / 2);
            position = Vector2.Zero;
            scale = new Vector2(1, 1);
            CalculateActiveArea();
            rotation = 0;
            duration = 0;
            scaleDuration = 0;
            followParentHorizontally = false;
            followParentVertically = false;
            rotationDestination = null;
            destination = null;
            scaleDestination = null;
            parent = null;
        }

        internal static void Move(Vector2 moveVector)
        {
            position -= moveVector;
            CalculateActiveArea();
        }

        internal static void SetPosition(Vector2 newPosition)
        {
            position = -newPosition;
            CalculateActiveArea();
        }

        internal static void SetScale(Vector2 newScale)
        {
            scale = newScale;
            CalculateActiveArea();
        }

        internal static void Scale(Vector2 scaleVector)
        {
            scale += scaleVector;
            if (scale.X < 0.01f)
            {
                scale = new Vector2(0.01f, scale.Y);
            }
            if (scale.Y < 0.01f)
            {
                scale = new Vector2(scale.X, 0.01f);
            }
            CalculateActiveArea();
            GameManager.game.WhenScaled();
        }

        public static void ScaleTo(Vector2 newScale, float duration)
        {
            scaleDestination = newScale;
            scaleDuration = duration;
        }

        public static void RotateTo(float newRotation, float duration)
        {
            rotationDestination = newRotation;
            rotationDuration = duration;
        }

        public static void Rotate(float step)
        {
            rotation += step;
        }

        public static void SetRotation(float newRotation)
        {
            rotation = newRotation;
        }

        private static void CalculateActiveArea()
        {
            int x = (int)(-(Camera.screenCenter.X) * ((1 / Camera.scale.X) - 1) + Camera.adjustedPosition.X) - 2;
            int y = (int)(-(Camera.screenCenter.Y) * ((1 / Camera.scale.Y) - 1) + Camera.adjustedPosition.Y) - 2;
            int width = (int)(((Camera.screenCenter.X * 2) * (1 / Camera.scale.X))) + 4;
            int height = (int)(((Camera.screenCenter.Y * 2) * (1 / Camera.scale.Y))) + 4;

            matrixBounds = new Rectangle(x, y, width, height);

            int x2 = (int)(-(Camera.screenCenter.X) * ((1 / Camera.scale.X * activeArea) - 1) + Camera.adjustedPosition.X) - 2;
            int y2 = (int)(-(Camera.screenCenter.Y) * ((1 / Camera.scale.Y * activeArea) - 1) + Camera.adjustedPosition.Y) - 2;
            int width2 = (int)(((Camera.screenCenter.X * 2) * (1 / Camera.scale.X)) * activeArea) + 4;
            int height2 = (int)(((Camera.screenCenter.Y * 2) * (1 / Camera.scale.Y)) * activeArea) + 4;

            activeBounds = new Rectangle(x2, y2, width2, height2);
        }

        public static void SetParent(GameObject newParent)
        {
            parent = newParent;
        }

        public static void MoveTo(Vector2 newPosition, float newDuration)
        {
            if (screenCenter != newPosition)
            {
                destination = newPosition;
                duration = newDuration;
            }
        }

        public static void MoveBy(Vector2 newPosition, float newDuration)
        {
            newPosition = adjustedPosition + screenCenter + newPosition;
            if (screenCenter != (newPosition - screenCenter))
            {
                destination = newPosition;
                duration = newDuration;
            }
        }

        public static void MoveByFrameWidth(int direction, float duration)
        {
            float width = matrixBounds.Width * direction;
            MoveBy(new Vector2(width, 0), duration);
        }

        public static string GetProperties()
        {
            string str = "";
            str += adjustedPosition + "|";
            str += scale + "|";
            return str;
        }

        public static void SetPropertiesAfterLoad(Vector2 adjustedPosition, Vector2 scale)
        {
            SetPosition(adjustedPosition);
            ScaleTo(scale, 0);
        }
        
    }
}
