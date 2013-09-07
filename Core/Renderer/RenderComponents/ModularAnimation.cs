using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Orujin.Core.Renderer
{
    public class ModularAnimation
    {
        public struct Frame
        {
            public float duration;
            public float rotation;
            public Vector2 scale;
            public Vector2 positionOffset;
        }

        private List<Frame> frames = new List<Frame>();
        private int currentFrame = 0;
        private float currentDuration = 0;

        public bool frozen = false;
        public bool playing = true;
        public bool reversing = false; //NOT IMPLEMENTED
        public float playRate = 1;

        public float threshold = 0;
        public float thresholdCounter = 0;

        public float rotation { get; private set; }
        public Vector2 scale { get; private set; }
        public Vector2 positionOffset { get; private set; }

        private float rotationMagnitude = 0;
        private Vector2 scaleMagnitude = Vector2.Zero;
        private Vector2 positionOffsetMagnitude = Vector2.Zero;

        public ModularAnimation(List<Frame> frames)
        {
            this.frames = frames;
            this.NewFrame(0);
        }

        public void Update(float elapsedTime)
        {
            if (this.playing && !this.frozen)
            {
                this.thresholdCounter += elapsedTime;
                if (this.thresholdCounter >= this.threshold)
                {
                    elapsedTime = this.thresholdCounter * this.playRate;
                    this.thresholdCounter = 0;
                    if (this.rotation != this.GetNextFrame(this.currentFrame).rotation)
                    {
                        this.HandleRotation(elapsedTime);
                    }
                    if (!this.scale.Equals(this.GetNextFrame(this.currentFrame).scale))
                    {
                        this.HandleScale(elapsedTime);
                    }
                    if (!this.positionOffset.Equals(this.GetNextFrame(this.currentFrame).positionOffset))
                    {
                        this.HandlePositionOffset(elapsedTime);
                    }

                    this.currentDuration -= elapsedTime;

                    if (this.currentDuration <= 0)
                    {
                        this.NewFrame(this.GetNextFrameIndex(this.currentFrame));
                    }
                }
            }
        }

        public void NewFrame(int index)
        {
            this.currentFrame = index;

            Frame currentFrame = frames[this.currentFrame];
            Frame nextFrame = this.GetNextFrame(this.currentFrame);
                this.rotation = currentFrame.rotation;
                this.scale = currentFrame.scale;
                this.positionOffset = currentFrame.positionOffset;

                this.currentDuration += currentFrame.duration;

                this.rotationMagnitude = (nextFrame.rotation - this.rotation) / this.currentDuration;
                this.scaleMagnitude = (nextFrame.scale - this.scale) / this.currentDuration;
                this.positionOffsetMagnitude = new Vector2(((nextFrame.positionOffset.X - this.positionOffset.X) / this.currentDuration),
                                                    (nextFrame.positionOffset.Y - this.positionOffset.Y) / this.currentDuration);

        }

        private void HandleRotation(float elapsedTime)
        {
            float currentRotation = elapsedTime * this.rotationMagnitude;
            this.rotation += currentRotation;

            float rotationDestination = this.GetNextFrame(this.currentFrame).rotation;
            if (rotationDestination >= this.rotation)
            {
                MathHelper.Clamp(this.rotation, float.MinValue, rotationDestination);
            }
            else
            {
                MathHelper.Clamp(this.rotation, rotationDestination, float.MaxValue);
            }
        }

        private void HandleScale(float elapsedTime)
        {
            Vector2 currentScale = elapsedTime * this.scaleMagnitude;
            this.scale += currentScale;

            Vector2 scaleDestination = this.GetNextFrame(this.currentFrame).scale;

            if (scaleDestination.X >= this.scale.X)
            {
                MathHelper.Clamp(this.scale.X, float.MinValue, scaleDestination.X);
            }
            else
            {
                MathHelper.Clamp(this.scale.X, scaleDestination.X, float.MaxValue);
            }

            if (scaleDestination.Y >= this.scale.Y)
            {
                MathHelper.Clamp(this.scale.Y, float.MinValue, scaleDestination.Y);
            }
            else
            {
                MathHelper.Clamp(this.scale.Y, scaleDestination.Y, float.MaxValue);
            }
        }

        private void HandlePositionOffset(float elapsedTime)
        {
            Vector2 currentPositionOffset = elapsedTime * this.positionOffsetMagnitude;
            this.positionOffset += currentPositionOffset;

            Vector2 positionOffsetDestination = this.GetNextFrame(this.currentFrame).positionOffset;
            if (positionOffsetDestination.X >= this.positionOffset.X)
            {
                MathHelper.Clamp(this.positionOffset.X, float.MinValue, positionOffsetDestination.X);
            }
            else
            {
                MathHelper.Clamp(this.positionOffset.X, positionOffsetDestination.X, float.MaxValue);
            }

            if (positionOffsetDestination.Y >= this.positionOffset.Y)
            {
                MathHelper.Clamp(this.positionOffset.Y, float.MinValue, positionOffsetDestination.Y);
            }
            else
            {
                MathHelper.Clamp(this.positionOffset.Y, positionOffsetDestination.Y, float.MaxValue);
            }
        }

        private Frame GetNextFrame(int currentFrame)
        {
            return this.frames[this.GetNextFrameIndex(currentFrame)];
        }

        private int GetNextFrameIndex(int currentFrame)
        {
            int nextFrame = currentFrame +1;
            if (nextFrame == this.frames.Count())
            {
                nextFrame = 0;
            }
            return nextFrame;
        }

        public void Reset()
        {
            this.currentFrame = 0;
            this.currentDuration = 0;
        }
    }
}
