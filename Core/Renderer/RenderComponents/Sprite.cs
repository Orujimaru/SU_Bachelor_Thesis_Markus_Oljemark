using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orujin.Framework;

namespace Orujin.Core.Renderer
{
    public class Sprite
    {
        private RendererComponents parent;
        public RendererPackage rendererPackage;
        public SpriteAnimation spriteAnimation { get; private set; }
        public ModularAnimation modularAnimation { get; private set; }
        private bool fadeOut = false;
        private bool fadeIn = false;
        private float fadeTime = 0;

        public string name { get; private set; }

        private Sprite(RendererComponents parent, Texture2D texture, Vector2 parentOffset, Vector2 scale, float rotation, Color color, int layer, string name, int overloadIndex)
        {
            this.parent = parent;
            this.rendererPackage.overloadIndex = overloadIndex;

            this.spriteAnimation = null;
            this.rendererPackage.visible = true;
            this.rendererPackage.texture = texture;
            this.rendererPackage.destination = new Rectangle(0, 0, texture.Bounds.Width, texture.Bounds.Height);
            this.rendererPackage.positionOffset = Vector2.Zero;
            this.rendererPackage.parentOffset = parentOffset;        
            this.rendererPackage.color = color;
            this.rendererPackage.originalColor = color;           
            this.rendererPackage.layer = layer;
            this.rendererPackage.rotation = rotation;
            this.rendererPackage.scale = scale;
            this.rendererPackage.defaultOrigin = new Vector2(this.rendererPackage.destination.Center.X, this.rendererPackage.destination.Center.Y);
            this.rendererPackage.origin = this.rendererPackage.defaultOrigin;
            this.rendererPackage.alpha = 1;
            this.rendererPackage.useforBounds = true;
            this.name = name;
            this.rendererPackage.debug = false;
        }

        private Sprite()
        {
        }

        public static Sprite CreateLight(RendererComponents parent, Texture2D texture, Vector2 position, Nullable<Vector2> origin, SpriteAnimation spriteAnimation, ModularAnimation ModularAnimation, Color color, string name)
        {
            Sprite s = new Sprite(parent, texture, position, new Vector2(1, 1), 0, color, RendererManager.LightLayer, name, 7);
            s.SetOrigin(origin);
            s.AddAnimation(spriteAnimation);
            s.AddModularAnimation(ModularAnimation);
            return s;
        }

        public static Sprite CreateSprite(RendererComponents parent, Texture2D texture, Vector2 position, Nullable<Vector2> origin, SpriteAnimation spriteAnimation, ModularAnimation ModularAnimation, Color color, string name)
        {
            Sprite s = new Sprite(parent, texture, position, new Vector2(1, 1), 0, color, 1, name, 2);
            s.SetOrigin(origin);
            s.AddAnimation(spriteAnimation);
            s.AddModularAnimation(ModularAnimation);
            return s;
        }

        public static Sprite CreateSprite(RendererComponents parent, Texture2D texture, Vector2 position, Vector2 scale, float rotation, bool flipH, bool flipV, Nullable<Vector2> origin, string name, int layer)
        {
            Sprite s = new Sprite(parent, texture, position, scale, rotation, Color.White, layer, name, 7);
            s.SetOrigin(origin);
            s.rendererPackage.flipHorizontally = flipH;
            s.rendererPackage.flipVertically = flipV;
            return s;
        }

        public static Sprite CreateColoredRectangle(RendererComponents parent, Color color, float width, float height, Vector2 position, string name, int layer)
        {
            Sprite s = new Sprite(parent, OrujinGame.GetTexture2DByColor(color), position, new Vector2(1, 1), 0, color, layer, name, 1);
            s.SetOrigin(new Vector2(width / 2, height / 2));
            s.rendererPackage.destination = new Rectangle(0, 0, (int)width, (int)height);
            return s;
        }

        public static Sprite CreateDebugText(RendererComponents parent, String text, Vector2 parentOffset, Color color)
        {
            Sprite s = new Sprite();
            s.name = "DebugText";
            s.rendererPackage.overloadIndex = 8;
            s.rendererPackage.layer = RendererManager.DebugLayer;
            s.rendererPackage.color = color;
            s.rendererPackage.parentOffset = parentOffset;
            s.rendererPackage.text = text;
            s.rendererPackage.debug = true;
            s.rendererPackage.visible = true;
            return s;
        }

        public void SetOrigin(Nullable<Vector2> newOrigin)
        {
            if (newOrigin == null)
            {
                this.rendererPackage.origin = this.rendererPackage.defaultOrigin;
            }
            else
            {
                this.rendererPackage.origin = (Vector2)newOrigin;
            }
        }

        public void AddAnimation(SpriteAnimation animation)
        {
            if (animation == null)
            {
                this.spriteAnimation = null;
            }
            else
            {
                this.spriteAnimation = animation;
                this.rendererPackage.origin = new Vector2(0, 0);
            }
            this.SetOverloadIndex();
        }

        public void AddModularAnimation(ModularAnimation animation)
        {
            if (animation == null)
            {
                this.spriteAnimation = null;
            }
            else
            {
                this.modularAnimation = animation;
            }
            this.SetOverloadIndex();
        }

        public void FadeIn(float time)
        {
            this.fadeIn = true;
            this.fadeOut = false;
            this.fadeTime = time;
        }

        public void FadeOut(float time)
        {
            this.fadeOut = true;
            this.fadeIn = false;
            this.fadeTime = time;
        }

        private void SetOverloadIndex()
        {
            if (this.spriteAnimation != null && this.modularAnimation == null)
            {
                this.rendererPackage.overloadIndex = 6;
            }
            else if (this.modularAnimation != null && this.spriteAnimation == null)
            {
                this.rendererPackage.overloadIndex = 7;
            }
            else if (this.modularAnimation != null)
            {
                this.rendererPackage.overloadIndex = 5;
            }    
            else
            {
                this.rendererPackage.overloadIndex = 2;
            }
        }

        public RendererPackage PrepareRendererPackage()
        {
            if (this.rendererPackage.texture != null)
            {
                if (this.spriteAnimation != null)
                {
                    Rectangle src = this.spriteAnimation.GetCurrentFrame();
                    this.rendererPackage.source = src;
                    this.rendererPackage.destination = new Rectangle(0,0, (int)(src.Width * this.rendererPackage.scale.X), (int)(src.Height * this.rendererPackage.scale.Y));
                }
                else
                {
                    this.rendererPackage.source = this.rendererPackage.texture.Bounds;
                }
                if (this.modularAnimation != null)
                {
                    this.rendererPackage.rotation = this.modularAnimation.rotation;
                    this.rendererPackage.scale = this.modularAnimation.scale;
                    this.rendererPackage.positionOffset = this.modularAnimation.positionOffset;
                }
            }
            return this.rendererPackage;
        }
    
        public void Update(float elapsedTime)
        {
            this.UpdateFade(elapsedTime);
            if (this.spriteAnimation != null)
            {
                this.spriteAnimation.Update(elapsedTime);
            }
            if (this.modularAnimation != null)
            {
                this.modularAnimation.Update(elapsedTime);
            }
        }

        private void UpdateFade(float elapsedTime)
        {
            if (this.fadeOut)
            {
                this.rendererPackage.alpha -= elapsedTime / fadeTime;
                if (this.rendererPackage.alpha <= 0)
                {
                    this.fadeOut = false;
                    this.rendererPackage.alpha = 0;
                    this.fadeTime = 0;
                }
            }
            else if (this.fadeIn) 
            {
                this.rendererPackage.alpha += elapsedTime / fadeTime;
                if (this.rendererPackage.alpha >= 1)
                {
                    this.fadeIn = false;
                    this.rendererPackage.alpha = 1;
                    this.fadeTime = 0;
                }
            }
        }

        public void AdjustBrightness(float newBrightness)
        {
            this.rendererPackage.color = Color.Lerp(Color.Black, this.rendererPackage.originalColor, newBrightness);
        }

        public Color[] GetColorData()
        {
            Rectangle source;
            if (this.spriteAnimation == null)
            {
                source = new Rectangle(0, 0, this.rendererPackage.texture.Bounds.Width, this.rendererPackage.texture.Bounds.Height);
            }
            else
            {
                source = this.rendererPackage.source;
            }

            Color[] colorData = new Color[source.Width * source.Height];
            this.rendererPackage.texture.GetData(0, source, colorData, source.X * source.Y, source.Width * source.Height);
            return colorData;
        }

        public void UpdateDebugText(string t)
        {
            this.rendererPackage.text = t;
        }

        public void FreezeAnimations()
        {
            if (this.spriteAnimation != null)
            {
                this.spriteAnimation.frozen = true;
            }
            if (this.modularAnimation != null)
            {
                this.modularAnimation.frozen = true;
            }
        }

        public void UnfreezeAnimations()
        {
            if (this.spriteAnimation != null)
            {
                this.spriteAnimation.frozen = false;
            }
            if (this.modularAnimation != null)
            {
                this.modularAnimation.frozen = false;
            }
        }
    }
}
