using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orujin.Core.Renderer
{
    public class SpriteAnimation
    {
        public struct Frame
        {
            public Rectangle source;
            public float duration;
        }

        public const float FinishedPlaying = -100000;
        public const int FirstFrame = -100;
        public const int LastFrame = 100;

        private List<Frame> frames;
        public int frameCount { get { return frames.Count; } private set { return; } }

        public int frameIndex { get; private set; }
        public float frameElapsedTime { get; private set; }

        public bool frozen = false;
        public bool playing = true;
        public bool reversing = false;
        public bool looping = true;
        public float playRate = 1;

        public SpriteAnimation(List<Frame> frames)
        {
            this.frames = frames;
            this.frameIndex = 0;
            this.frameElapsedTime = 0;
        }

        public void Update(float elapsedTime)
        {      
            if(this.frameElapsedTime != FinishedPlaying && this.playing && !this.frozen)
            {
                elapsedTime *= this.playRate;
                this.frameElapsedTime += elapsedTime;
                if (this.frameElapsedTime >= this.frames[this.frameIndex].duration)
                {
                    this.frameElapsedTime -= this.frames[this.frameIndex].duration;
                    if (this.reversing)
                    {
                        this.frameIndex--;
                        if (this.frameIndex == -1)
                        {
                            if (this.looping)
                            {
                                this.frameIndex = this.frames.Count() - 1;
                            }
                            else
                            {
                                this.frameIndex = 0;
                                this.frameElapsedTime = FinishedPlaying;
                            }
                        }
                    }
                    else
                    {                    
                        this.frameIndex++;
                        if (this.frameIndex == this.frames.Count())
                        {
                            if (this.looping)
                            {
                                this.frameIndex = 0;
                            }
                            else
                            {
                                this.frameIndex = this.frames.Count() - 1;
                                this.frameElapsedTime = FinishedPlaying;
                            }
                        }
                    }
                }
            }
        }
           

        public Rectangle GetCurrentFrame()
        {
            return this.frames[this.frameIndex].source;
        }

        public int SetFrame(int newFrame)
        {
            if (newFrame == FirstFrame)
            {
                newFrame = 0;
            }
            else if (newFrame == LastFrame)
            {
                newFrame = this.frames.Count() - 1;
            }

            if (newFrame < 0 || newFrame > this.frames.Count() - 1)
            {
                return 0;
            }
            else
            {
                this.frameIndex = newFrame;
                this.frameElapsedTime = 0;
                return 1;
            }
        }

        public void Reset()
        {
            if (this.reversing)
            {
                this.SetFrame(LastFrame);
            }
            else
            {
                this.SetFrame(FirstFrame);
            }
        }

        public bool HasFinished()
        {
            return (this.frameElapsedTime == FinishedPlaying);
        }
    }
}
