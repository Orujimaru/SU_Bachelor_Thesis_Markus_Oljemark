using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orujin.Debug;

namespace Orujin.Core.Renderer
{
    internal class Renderer
    {
        private RenderTarget2D lightTarget;
        private RenderTarget2D lightBlockerTarget;
        private RenderTarget2D debugTarget;
        private RenderTarget2D hudTarget;
        private RenderTarget2D menuTarget;

        private Vector2 RenderTargetPosition = new Vector2(0, 0);
        private SpriteBatch spriteBatch;
        private GraphicsDeviceManager graphics;

        private Texture2D ambientLight;

        private Effect effect = null;

        public static int width { get; private set; }
        public static int height { get; private set; }

        private SpriteFont font;

        public Renderer(ref GraphicsDeviceManager graphics, SpriteFont font, int frameWidth, int frameHeight)
        {
            width = frameWidth;
            height = frameHeight;
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();

            this.font = font;

            this.spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            this.lightTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            this.lightBlockerTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            this.debugTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            this.hudTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferWidth);
            this.menuTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferWidth);

            this.graphics = graphics;
            this.SetAmbientLight(1);
        }

        public void SetEffect(Effect effect)
        {
            this.effect = effect;
        }

        public void SetAmbientLight(float intensity)
        {
            this.ambientLight = new Texture2D(this.graphics.GraphicsDevice, 1, 1);
            Color[] data = new Color[1];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = new Color(Color.White, intensity);
            }

            this.ambientLight.SetData(data);
        }

        public void RenderLevel(List<RendererPackage> objects)
        {
            //Set the RenderTarget to the backbuffer
            this.graphics.GraphicsDevice.SetRenderTarget(null);
            this.graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            this.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.matrix);

            foreach (RendererPackage rp in objects)
            {
                this.RenderPackage(rp);
            }

            this.spriteBatch.End();
        }

        public void RenderLights(List<RendererPackage> lights)
        {
            this.graphics.GraphicsDevice.SetRenderTarget(this.lightTarget);
            this.graphics.GraphicsDevice.Clear(Color.Black);
            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Camera.matrix);

            //Draw ambient light to the active camera bounds
            this.spriteBatch.Draw(this.ambientLight, Camera.matrixBounds, Color.White);

            foreach (RendererPackage rp in lights)
            {
                this.RenderPackage(rp);
            }
            this.spriteBatch.End();
        }

        public void RenderLightBlockers(List<RendererPackage> lightBlockers)
        {
           
        }

        public void RenderDebug(List<RendererPackage> debug)
        {
            this.graphics.GraphicsDevice.SetRenderTarget(this.debugTarget);
            this.graphics.GraphicsDevice.Clear(Color.Transparent);
            this.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.matrix);

            foreach (RendererPackage rp in debug)
            {
                this.RenderPackage(rp);
            }

            this.spriteBatch.End();
        }

        public void RenderHUD(List<RendererPackage> hudItems)
        {
            this.graphics.GraphicsDevice.SetRenderTarget(this.hudTarget);
            this.graphics.GraphicsDevice.Clear(Color.Transparent);
            Matrix m = Matrix.CreateTranslation(new Vector3(-Camera.adjustedPosition, 0));

            this.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, m);

            foreach (RendererPackage rp in hudItems)
            {
                this.RenderPackage(rp);
            }

            this.spriteBatch.End();
        }

        public void RenderMenu(List<RendererPackage> menuItems)
        {
            this.graphics.GraphicsDevice.SetRenderTarget(this.menuTarget);
            this.graphics.GraphicsDevice.Clear(Color.Transparent);
            Matrix m = Matrix.CreateTranslation(new Vector3(-Camera.adjustedPosition, 0));

            this.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, m);

            foreach (RendererPackage rp in menuItems)
            {
                this.RenderPackage(rp);
            }

            this.spriteBatch.End();
        }

        private void RenderPackage(RendererPackage rp)
        {
            if (!rp.visible)
            {
                return;
            }

            Color ac = rp.color * rp.alpha;

            //Render the RenderPackage by the right method overload
            switch(rp.overloadIndex)
            {
                case 1:
                {
                    this.spriteBatch.Draw(rp.texture, rp.destination, ac);
                    break;
                }
                case 2:
                {
                    this.spriteBatch.Draw(rp.texture, rp.position, ac);
                    break;
                }
                case 3:
                {
                    this.spriteBatch.Draw(rp.texture, rp.destination, rp.source, ac);
                    break;
                }
                case 4:
                {
                    this.spriteBatch.Draw(rp.texture, rp.position, rp.source, ac);
                    break;
                }
                case 5:
                {
                    this.spriteBatch.Draw(rp.texture, rp.destination, rp.source, ac, rp.rotation, rp.origin, rp.spriteEffects, 0);
                    break;
                }
                case 6:
                {
                    this.spriteBatch.Draw(rp.texture, rp.position, rp.source, ac, rp.rotation, rp.origin, rp.scale.X, rp.spriteEffects, 0);
                    break;
                }
                case 7:
                {
                    this.spriteBatch.Draw(rp.texture, rp.position, rp.source, ac, rp.rotation, rp.origin, rp.scale, rp.spriteEffects, 0);
                    break;
                }
                case 8:
                {
                    this.spriteBatch.DrawString(this.font, rp.text, rp.position, rp.color);
                    break;
                }
            }           
        }

        public void Finish()
        {
            BlendState blendState = new BlendState();
            blendState.AlphaDestinationBlend = Blend.SourceColor;
            blendState.ColorDestinationBlend = Blend.SourceColor;
            blendState.AlphaSourceBlend = Blend.Zero;
            blendState.ColorSourceBlend = Blend.Zero;

            //Draw the additional RenderTarget2D on top of the back buffer.
            spriteBatch.Begin(SpriteSortMode.Deferred, blendState, null, null, null);
            spriteBatch.Draw(this.lightTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            spriteBatch.Draw(this.hudTarget, Vector2.Zero, Color.White);
            spriteBatch.Draw(this.menuTarget, Vector2.Zero, Color.White);

            if (DebugManager.visibleDebug)
            {
                spriteBatch.Draw(this.debugTarget, Vector2.Zero, Color.White);
            }

            spriteBatch.End();
        }
    }

    public struct RendererPackage
    {
        public bool useforBounds;
        public int overloadIndex;
        public bool visible;
        public Texture2D texture
        {
            get
            {
                if(this.gray)
                {
                    if (this.grayTexture == null)
                    {
                        CreateGrayscale();
                    }
                    return this.grayTexture;
                }
                return this.normalTexture;
            }
            set
            {  
                this.gray = false;
                this.normalTexture = value;
            }
        }
        
        private bool gray;
        private Texture2D normalTexture;
        private Texture2D grayTexture;

        private Vector2 privatePosition;
        public Vector2 position
        {
            get
            {
                return this.privatePosition;
            }
            set { this.privatePosition = value; }
        }

        //Used by modular animation
        public Vector2 positionOffset;

        public Vector2 parentOffset;
        
        public Rectangle destination;

        public Rectangle source;
        public Color color;
        public Color originalColor;

        internal Vector2 defaultOrigin;
        public Vector2 origin;

        public float rotation;
        public float alpha;
        public Vector2 scale;
        public bool flipHorizontally;
        public bool flipVertically;

        public SpriteEffects spriteEffects
        {
            get
            {
                SpriteEffects temp = SpriteEffects.None;
                if (this.flipHorizontally && this.flipVertically)
                {
                    temp = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                }
                else if (this.flipHorizontally)
                {
                    temp = SpriteEffects.FlipHorizontally;
                }
                else if (this.flipVertically)
                {
                    temp = SpriteEffects.FlipVertically;
                }
                return temp;
            }
            private set { return; }
        }
        public int layer;
        
        public void ToggleGrayTexture()
        {
            if(this.grayTexture == null)
            {
                this.CreateGrayscale();
            }
            this.gray = true;
        }
        
        public void ToggleNormalTexture()
        {
            this.gray = false;
        }
        
        /***Invalid***/
        public void CreateGrayscale()
        {
            this.grayTexture = this.normalTexture; 
            Color[] color = new Color[this.grayTexture.Width*grayTexture.Height]; 
            this.grayTexture.GetData<Color>(color); 
            for (int i = 0; i < this.grayTexture.Width * this.grayTexture.Height; i++) 
            {
                color[i] = Color.Lerp(color[i], Color.GhostWhite, 0.75f);
            }
            this.grayTexture.SetData<Color>(color);
        }

        //Used for debugging
        public bool debug;
        public string text;
    }

    /***This class currently does not serve its purpose***/
    internal static class Utilities
    {

        public static void IncreaseHueBy(ref Color color, float value, out float hue)
        {
            float h, s, v;

            RgbToHsv(color.R, color.G, color.B, out h, out s, out v);
            h += value;

            float r, g, b;

            HsvToRgb(h, s, v, out r, out g, out b);


            color.R = (byte)(r);
            color.G = (byte)(g);
            color.B = (byte)(b);

            hue = h;
        }

        static void RgbToHsv(float r, float g, float b, out float h, out float s, out float v)
        {
            float min, max, delta;
            min = System.Math.Min(System.Math.Min(r, g), b);
            max = System.Math.Max(System.Math.Max(r, g), b);
            v = max;               // v
            delta = max - min;
            if (max != 0)
            {
                s = delta / max;       // s

                if (r == max)
                    h = (g - b) / delta;       // between yellow & magenta
                else if (g == max)
                    h = 2 + (b - r) / delta;   // between cyan & yellow
                else
                    h = 4 + (r - g) / delta;   // between magenta & cyan
                h *= 60;               // degrees
                if (h < 0)
                    h += 360;
            }
            else
            {
                // r = g = b = 0       // s = 0, v is undefined
                s = 0;
                h = -1;
            }

        }
        static void HsvToRgb(float h, float s, float v, out float r, out float g, out float b)
        {
            // Keeps h from going over 360
            h = h - ((int)(h / 360) * 360);

            int i;
            float f, p, q, t;
            if (s == 0)
            {
                // achromatic (grey)
                r = g = b = v;
                return;
            }
            h /= 60;           // sector 0 to 5

            i = (int)h;
            f = h - i;         // factorial part of h
            p = v * (1 - s);
            q = v * (1 - s * f);
            t = v * (1 - s * (1 - f));
            switch (i)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                default:       // case 5:
                    r = v;
                    g = p;
                    b = q;
                    break;
            }
        }
    }
}
