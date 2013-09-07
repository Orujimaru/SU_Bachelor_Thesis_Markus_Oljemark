using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orujin.Framework;

namespace Orujin.Core.Renderer
{
    public class RendererComponents
    {
        private List<Sprite> children;
        private bool debugging = false;
        private Texture2D debugTexture = null;
        public GameObject parent = null;
        private AnimationJuggler animationJuggler = null;
        public Rectangle bounds { get; private set; }

        public Vector2 cloneOffset { get; private set; }

        public RendererComponents(GameObject parent)
        {
            this.parent = parent;
            this.children = new List<Sprite>();
            this.cloneOffset = Vector2.Zero;
            this.bounds = new Rectangle((int)parent.origin.X, (int)parent.origin.Y, 2, 2);
        }

        public void SetCloneOffset(Vector2 cloneOffset)
        {
            this.cloneOffset = cloneOffset;
        }

        public List<RendererPackage> GetRendererPackages()
        {
            List<RendererPackage> rendererPackages = new List<RendererPackage>();

            List<Sprite> tempChildren = this.GetChildren();

            foreach (Sprite sprite in tempChildren)
            {
                rendererPackages.Add(sprite.rendererPackage);
            }
            if (this.cloneOffset != Vector2.Zero)
            {
                List<Sprite> tempClones = this.GetClones();
                foreach (Sprite sprite in tempClones)
                {
                    rendererPackages.Add(sprite.rendererPackage);
                }
            }

            //Create a RendererPackage for the origin if debugging is true
            if (this.debugging)
            {
                RendererPackage debugPackage = new RendererPackage();
                debugPackage.texture = this.debugTexture;
                debugPackage.destination = new Rectangle((int)(this.parent.origin.X - 5), (int)(this.parent.origin.Y - 5), 10, 10);
                debugPackage.overloadIndex = 1;
                debugPackage.layer = 100;
                debugPackage.color = Color.Black;
                rendererPackages.Add(debugPackage);
            }
            return rendererPackages;
        }

        public List<Sprite> GetChildren()
        {
            foreach (Sprite sprite in this.children)
            {
                sprite.PrepareRendererPackage();
                float widthOffset = sprite.rendererPackage.destination.Width / 2;
                float heightOffset = sprite.rendererPackage.destination.Height / 2;

                Vector2 parentOffset = sprite.rendererPackage.parentOffset;

                Vector2 originOffset = new Vector2(widthOffset, heightOffset);
                if (sprite.rendererPackage.overloadIndex >= 5)
                {
                    originOffset = originOffset - sprite.rendererPackage.origin;             
                }
                sprite.rendererPackage.position = this.parent.origin - originOffset + parentOffset + sprite.rendererPackage.positionOffset;
                sprite.rendererPackage.destination = new Rectangle((int)sprite.rendererPackage.position.X, (int)sprite.rendererPackage.position.Y, sprite.rendererPackage.destination.Width, sprite.rendererPackage.destination.Height);                         
            }
            return this.children;
        }

        public List<Sprite> GetClones()
        {
            List<Sprite> clones = new List<Sprite>(this.children);
            foreach (Sprite sprite in clones)
            {
                sprite.rendererPackage.position += cloneOffset;  
            }
            return clones;
        }

        public Sprite GetChildByName(String name)
        {
            foreach (Sprite sprite in this.children)
            {
                if (sprite.name.Equals(name))
                {
                    sprite.PrepareRendererPackage();
                    float widthOffset = sprite.rendererPackage.destination.Width / 2;
                    float heightOffset = sprite.rendererPackage.destination.Height / 2;

                    Vector2 parentOffset = sprite.rendererPackage.parentOffset;

                    Vector2 originOffset = new Vector2(widthOffset, heightOffset);
                    if (sprite.rendererPackage.overloadIndex >= 5)
                    {
                        originOffset = originOffset - sprite.rendererPackage.origin;
                    }

                    sprite.rendererPackage.position = this.parent.origin - originOffset + parentOffset + sprite.rendererPackage.positionOffset;
                    sprite.rendererPackage.destination = new Rectangle((int)sprite.rendererPackage.position.X, (int)sprite.rendererPackage.position.Y, sprite.rendererPackage.destination.Width, sprite.rendererPackage.destination.Height);
                    return sprite;
                }
            }
            return null;
        }

        public void Update(float elapsedTime)
        {
            if (this.animationJuggler == null)
            {
                foreach (Sprite s in this.children)
                {
                    s.Update(elapsedTime);
                }
            }
            else
            {
                this.animationJuggler.Update(this.children, elapsedTime);
            }
        }

        public void FadeOut(float time)
        {
            foreach (Sprite s in this.children)
            {
                s.FadeOut(time);
            }
        }

        public float AverageAlpha()
        {
            float alpha = 0;
            foreach (Sprite s in this.children)
            {
                alpha += s.rendererPackage.alpha;
            }
            alpha /= this.children.Count;
            return alpha;
        }

        public void FadeIn(float time)
        {
            foreach (Sprite s in this.children)
            {
                s.FadeIn(time);
            }
        }

        public void FlipChildren(bool horizontally, bool vertically)
        {
            foreach (Sprite s in this.children)
            {
                s.rendererPackage.flipHorizontally = horizontally;
                s.rendererPackage.flipVertically = vertically;
            }
        }
        public void AddLight(Texture2D texture, Vector2 offset, Nullable<Vector2> origin, Color color, string name)
        {
            this.children.Add(Sprite.CreateLight(this, texture, offset, origin, null, null, color, name));
        }

        public void AddGameSprite(Texture2D texture, Vector2 offset, Nullable<Vector2> origin, Color color, string name)
        {
            this.children.Add(Sprite.CreateSprite(this, texture, offset, origin, null, null, color, name));
        }

        public void AddGameSprite(Texture2D texture, Vector2 offset, Vector2 scale, float rotation, bool flipH, bool flipV, Nullable<Vector2> origin, Color color, string name)
        {
            //this.children.Add(Sprite.CreateSprite(texture, offset, origin, null, null, color, name));
            this.children.Add(Sprite.CreateSprite(this, texture, offset, scale, rotation, flipH, flipV, origin, name, RendererManager.ObjectLayer));
        }

        public void AddMenuItem(Texture2D texture, Vector2 offset, Nullable<Vector2> origin, Color color, string name)
        {
            Sprite s = Sprite.CreateSprite(this, texture, offset, origin, null, null, color, name);
            s.rendererPackage.layer = RendererManager.MenuLayer;
            this.children.Add(s);
        }

        public void AddHudItem(Texture2D texture, Vector2 offset, Vector2 scale, float rotation, bool flipH, bool flipV, Nullable<Vector2> origin, Color color, string name)
        {
            Sprite s = Sprite.CreateSprite(this, texture, offset, scale, rotation, flipH, flipV, origin, name, RendererManager.HudLayer);
            this.children.Add(s);
        }

        public void AddGameSprite(Sprite sprite)
        {
            this.children.Add(sprite);
        }

        public void AddColoredRectangleForLevel(Color color, float width, float height, Vector2 offset, Nullable<Vector2> origin, string name)
        {
            this.children.Add(Sprite.CreateColoredRectangle(this, color, width, height, offset, name, RendererManager.ObjectLayer));
        }

        public void AddColoredRectangleForHud(Color color, float width, float height, Vector2 offset, Nullable<Vector2> origin, string name)
        {
            this.children.Add(Sprite.CreateColoredRectangle(this, color, width, height, offset, name, RendererManager.HudLayer));
        }

        public void AddColoredRectangleForMenu(Color color, float width, float height, Vector2 offset, Nullable<Vector2> origin, string name)
        {
            this.children.Add((Sprite.CreateColoredRectangle(this, color, width, height, offset, name, RendererManager.MenuLayer)));
        }

        public void AddAnimationJuggler(AnimationJuggler juggler)
        {
            this.animationJuggler = juggler;
        }

        public int AddAnimationToComponent(string componentName, SpriteAnimation animation)
        {
            for (int x = 0; x < this.children.Count(); x++)
            {
                if (this.children[x].name == componentName)
                {
                    this.children[x].AddAnimation(animation);
                    return 1;
                }
            }
            return 0;
        }

        public int AddAnimationToComponent(string componentName, SpriteAnimation animation, bool play, bool repeat)
        {
            for (int x = 0; x < this.children.Count(); x++)
            {
                if (this.children[x].name == componentName)
                {
                    this.children[x].AddAnimation(animation);
                    this.children[x].spriteAnimation.playing = play;
                    this.children[x].spriteAnimation.looping = repeat;
                    return 1;
                }
            }
            return 0;
        }

        public int AddModularAnimationToComponent(string componentName, ModularAnimation animation)
        {
            for (int x = 0; x < this.children.Count(); x++)
            {
                if (this.children[x].name == componentName)
                {
                    this.children[x].AddModularAnimation(animation);
                    return 1;
                }
            }       
            return 0;
        }

        public void AdjustBrightness(float newBrightness)
        {
            for (int x = 0; x < this.children.Count(); x++)
            {
                this.children[x].AdjustBrightness(newBrightness);
            }
        }

        public void Debug(Texture2D texture)
        {
            this.debugging = true;
            this.debugTexture = texture;            
        }
        public void StopDebugging()
        {
            this.debugging = false;
            this.debugTexture = null;
        }

        public void ToggleVisibilityOn()
        {
            foreach (Sprite s in this.children)
            {
                s.rendererPackage.visible = true;
            }
        }

        public void ToggleVisibilityOff()
        {
            foreach (Sprite s in this.children)
            {
                s.rendererPackage.visible = false;
            }
        }

        public void SetBounds()
        {
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;

            foreach(Sprite s in this.GetChildren())
            {
                if (!s.rendererPackage.debug && s.rendererPackage.useforBounds)
                {
                    float widthOffset = s.rendererPackage.destination.Width / 2 * s.rendererPackage.scale.X;
                    float heightOffset = s.rendererPackage.destination.Height / 2 * s.rendererPackage.scale.Y;

                    Vector2 parentOffset = s.rendererPackage.parentOffset;

                    Vector2 originOffset = new Vector2(widthOffset, heightOffset);
                    if (s.rendererPackage.overloadIndex >= 5)
                    {
                        originOffset = originOffset - s.rendererPackage.origin;
                    }

                    Vector2 spritePosition = this.parent.origin - originOffset + parentOffset + s.rendererPackage.positionOffset;

                    minX = Math.Min(minX, (int)spritePosition.X);
                    maxX = Math.Max(maxX, (int)spritePosition.X + (int)widthOffset* 2);

                    minY = Math.Min(minY, (int) spritePosition.Y);
                    maxY = Math.Max(maxY, (int) spritePosition.Y + (int) heightOffset * 2);
                }
            }
            this.bounds = new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public Sprite AddDebugText(string text, Vector2 parentOffset, Color color)
        {
            Sprite s = Sprite.CreateDebugText(this, text, parentOffset, color);
            this.children.Add(s);
            return s;
        }

        public void FreezeAllAnimations()
        {
            foreach (Sprite s in this.children)
            {
                s.FreezeAnimations();
            }
        }

        public void UnfreezeAllAnimations()
        {
            foreach (Sprite s in this.children)
            {
                s.UnfreezeAnimations();
            }
        }

        public void ClearChildren()
        {
            this.children.Clear();
        }
    }
}
