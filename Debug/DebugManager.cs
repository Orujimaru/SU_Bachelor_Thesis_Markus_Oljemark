using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Orujin.Core.Renderer;
using Orujin.Framework;

namespace Orujin.Debug
{
    public class DebugManager
    {
        private FarseerDebugView farseerDebugView;
        private bool visibleFarseer = false;
        public static bool visibleDebug = false;

        public DebugManager()
        {
        }

        public void InitiateFarseerDebugView(GraphicsDevice graphics, ContentManager content, World world)
        {
            // create and configure the debug view
            this.farseerDebugView = new FarseerDebugView(world);
            this.farseerDebugView.AppendFlags(DebugViewFlags.DebugPanel);
            this.farseerDebugView.DefaultShapeColor = Color.White;
            this.farseerDebugView.SleepingShapeColor = Color.LightGray;
            this.farseerDebugView.LoadContent(graphics, content);
        }

        public void RenderFarseerDebugView(GraphicsDeviceManager graphics)
        {
            if (this.visibleFarseer)
            {
                // calculate the projection and view adjustments for the debug view
                Matrix projection = Matrix.CreateOrthographicOffCenter(0f, graphics.PreferredBackBufferWidth / Camera.PixelsPerMeter,
                                                                 graphics.PreferredBackBufferHeight * 1.0001f / Camera.PixelsPerMeter, 0f, 0f,
                                                                 1f);
                Matrix view = Camera.farseerMatrix;

                // draw the debug view
                this.farseerDebugView.RenderDebugData(ref projection, ref view);
            }
        }

        public void ToggleVisibleFarseer(int data)
        {
            this.visibleFarseer = !this.visibleFarseer;
        }
        public void ToggleVisibleDebug(int data)
        {
            visibleDebug = !visibleDebug;
        }

        public void DebugPlayMusic(string music)
        {
            OrujinGame.PlaySong(music, false);
        }
    }
}
