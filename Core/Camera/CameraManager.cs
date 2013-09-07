using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Orujin.Framework;

namespace Orujin.Core.Renderer
{
    public class CameraManager
    {
        public bool centerParentHorizontally = false;
        public bool centerParentVertically = false;
        public bool centerOnParentContiniously { get; private set; }
        public float delay { get; private set; }
        private static int shakeTimes = 0;
        private static Vector2 previousShakeDirection = Vector2.Zero;

        public CameraManager(float frameWidth, float frameHeight)
        {
            this.centerOnParentContiniously = false;
            this.delay = 0;
            Camera.Initialize(frameWidth, frameHeight);
        }

        public void SetPosition(Vector2 position)
        {
            Camera.SetPosition(position);
        }

        public void Move(Vector2 moveVector)
        {
            Camera.Move(moveVector);
        }

        public static void Shake(int times)
        {
            shakeTimes = times;
        }

        public void MoveBy(Vector2 moveVector, float duration)
        {
            Camera.MoveBy(moveVector, duration);
        }

        public void MoveByFrameWidth(int direction, float duration)
        {
            Camera.MoveByFrameWidth(direction, duration);
        }

        public void MoveTo(Vector2 destination, float duration)
        {
            Camera.MoveTo(destination, duration);
        }

        public void UnlockHorizontally(int delay)
        {
            if (Camera.parent != null)
            {
                this.delay = delay;
                this.centerParentHorizontally = true;
                this.centerOnParentContiniously = true;
            }
        }

        public void LockHorizontally(int data)
        {
            this.centerParentHorizontally = false;
            if (!this.centerParentVertically)
            {
                this.centerOnParentContiniously = false;
            }
        }

        public void UnlockVertically(int delay)
        {
            if (Camera.parent != null)
            {
                this.delay = delay;
                this.centerParentVertically = true;
                this.centerOnParentContiniously = true;
            }
        }

        public void LockVertically(int data)
        {
            this.centerParentVertically = false;
            if (!this.centerParentHorizontally)
            {
                this.centerOnParentContiniously = false;
            }
        }

        public void SetParent(GameObject parent)
        {
            Camera.SetParent(parent);
        }

        public void CenterParent(float duration)
        {
            if (Camera.parent != null)
            {
                Camera.MoveTo(Camera.parent.origin, duration);
            }
        }

        public void CenterParentContiniously(float delay)
        {
            if (Camera.parent != null)
            {
                this.delay = delay;
                this.centerParentHorizontally = true;
                this.centerParentVertically = true;
                this.centerOnParentContiniously = true;
            }
        }

        public void StopCenteringParentContiniously()
        {
            this.centerOnParentContiniously = false;
            this.CenterParent(delay);
            this.delay = 0;
        }

        public void Scale(Vector2 scale)
        {
            Camera.Scale(scale);
        }

        public void ScaleTo(Vector2 scaleTarget, float duration)
        {
            Camera.ScaleTo(scaleTarget, duration);
        }

        public void Update(float elapsedTime)
        {
            if (shakeTimes > 0)
            {
                if (previousShakeDirection == Vector2.Zero)
                {
                    previousShakeDirection = new Vector2(0, 4);
                }
                else if (previousShakeDirection == new Vector2(0, 4))
                {
                    previousShakeDirection = new Vector2(0, -4);
                }
                else if (previousShakeDirection == new Vector2(0, -4))
                {
                    previousShakeDirection = Vector2.Zero;
                    shakeTimes--;
                }
                Camera.MoveBy(previousShakeDirection, 1);
            }
            if (Camera.scaleDestination != null && OrujinGame.orujin.updateLogic)
            {
                Vector2 scaleDestination = Camera.scaleDestination.Value;
                float scaleDuration = Camera.scaleDuration;

                Vector2 offset = scaleDestination - (Camera.scale);
                Vector2 vector = (offset / scaleDuration) * elapsedTime;

                scaleDuration -= elapsedTime;

                if (scaleDuration <= 0)
                {
                    Camera.SetScale(scaleDestination);
                    Camera.scaleDestination = null;
                    Camera.scaleDuration = 0;
                }
                else
                {
                    Camera.scaleDuration = scaleDuration;
                    Camera.Scale(vector);
                }
            }

            if (Camera.rotationDestination != null)
            {
                float rotationDestination = Camera.rotationDestination.Value;
                float rotationDuration = Camera.rotationDuration;

                float offset = rotationDestination - Camera.rotation;
                float step = (offset / rotationDuration) * elapsedTime;
                
                rotationDuration -= elapsedTime;

                if (rotationDuration <= 0)
                {
                    Camera.rotation = Camera.rotationDestination.Value;
                    Camera.rotationDestination = null;
                    Camera.rotationDuration = 0;
                }
                else
                {
                    Camera.rotationDuration = rotationDuration;
                    Camera.Rotate(step);
                }

            }

            if (Camera.destination != null || this.centerOnParentContiniously)
            {
                if (this.centerOnParentContiniously)
                {
                    if (this.centerParentVertically && this.centerParentHorizontally)
                    {
                        Camera.destination = new Vector2(Camera.parent.origin.X, Camera.parent.origin.Y);
                    }
                    else if (this.centerParentHorizontally)
                    {
                        Camera.destination = new Vector2(Camera.parent.origin.X, Camera.adjustedPosition.Y + Camera.screenCenter.Y);
                    }
                    else if(this.centerParentVertically)
                    {
                        Camera.destination = new Vector2(Camera.adjustedPosition.X + Camera.screenCenter.X, Camera.parent.origin.Y);
                    }
                }

                Vector2 destination = Camera.destination.Value;
                float duration = Camera.duration;

                if (this.centerOnParentContiniously)
                {
                    duration = this.delay;
                }

                Vector2 distance = destination - (Camera.adjustedPosition + Camera.screenCenter);
                Vector2 vector = (distance / duration) * elapsedTime;

                duration -= elapsedTime;

                if (duration <= 0)
                {
                    Camera.SetPosition(destination - Camera.screenCenter);
                    Camera.destination = null;
                    Camera.duration = 0;
                }
                else
                {
                    Camera.duration = duration;
                    Camera.Move(vector);
                }

            }
        }
    }
}
