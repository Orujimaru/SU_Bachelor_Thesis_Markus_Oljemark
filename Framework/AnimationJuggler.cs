using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orujin.Core.Renderer;

namespace Orujin.Framework
{
    public class AnimationJuggler
    {
        public int queuedAnimation { get; protected set; }
        public int queuedFrame { get; protected set; }
        public int activeAnimation { get; private set; }
        protected bool newAnimation = false;
        protected int newAnimationFrame = 0;

        public AnimationJuggler(int animationState)
        {
            this.activeAnimation = animationState;
            this.queuedAnimation = 0;
        }

        public virtual void SetAnimation(int animation, int frame)
        {
            this.activeAnimation = animation;
            this.newAnimationFrame = frame;
            this.newAnimation = true;
        }

        public virtual void Update(List<Sprite> sprites, float elapsedTime)
        {
        }

        public void QueueAnimation(int animation, int frame)
        {
            this.queuedAnimation = animation;
            this.queuedFrame = frame;
        }

        protected void PrepareAnimation(Sprite s)
        {
            if (this.newAnimationFrame == 0)
            {
                s.spriteAnimation.Reset();
            }
            else
            {
                s.spriteAnimation.SetFrame(this.newAnimationFrame);
            }
            s.rendererPackage.visible = true;
            this.newAnimation = false;
        }
    }
}
