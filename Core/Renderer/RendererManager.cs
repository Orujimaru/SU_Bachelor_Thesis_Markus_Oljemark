﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orujin.Core.Logic;
using Orujin.Debug;
using Orujin.Framework;

namespace Orujin.Core.Renderer
{
    public class RendererManager
    {
        public const int ObjectLayer = 1;
        public const int LightLayer = 2;
        public const int LightBlockerLayer = 3;
        public const int MenuLayer = 4;
        public const int HudLayer = 5;
        public const int DebugLayer = 100;

        private Renderer renderer;

        public float ambientLightIntensity { get; private set; }

        private float targetAmbientLightIntensity = 1;
        private float fadeMagnitude = 0;

        private List<RendererPackage> objects;
        private List<RendererPackage> lights;
        private List<RendererPackage> lightBlockers;
        private List<RendererPackage> debug;
        private List<RendererPackage> menuItems;
        private List<RendererPackage> hudItems;

        public RendererManager(ref GraphicsDeviceManager graphics, SpriteFont font, int frameWidth, int frameHeight)
        {
            this.renderer = new Renderer(ref graphics, font, frameWidth, frameHeight);

            this.objects = new List<RendererPackage>();
            this.lights = new List<RendererPackage>();
            this.lightBlockers = new List<RendererPackage>();
            this.debug = new List<RendererPackage>();
            this.menuItems = new List<RendererPackage>();
            this.hudItems = new List<RendererPackage>();
        }

        public void Update(float elapsedTime)
        {
            if (this.fadeMagnitude != 0)
            {
                this.ambientLightIntensity += this.fadeMagnitude * elapsedTime;
                if (this.fadeMagnitude > 0)
                {
                    if (this.ambientLightIntensity >= this.targetAmbientLightIntensity)
                    {
                        this.ambientLightIntensity = this.targetAmbientLightIntensity;
                        this.fadeMagnitude = 0;
                    }
                }
                else
                {
                    if (this.ambientLightIntensity <= this.targetAmbientLightIntensity)
                    {
                        this.ambientLightIntensity = this.targetAmbientLightIntensity;
                        this.fadeMagnitude = 0;
                    }
                }
                this.SetAmbientLightIntensity(this.ambientLightIntensity);
            }
        }

        public void SetAmbientLightIntensity(float intensity)
        {
            this.ambientLightIntensity = intensity;
            renderer.SetAmbientLight(MathHelper.Clamp(intensity, -1, 1));
        }

        public void SetEffect(Effect effect)
        {
            renderer.SetEffect(effect);
        }

        public void FadeAmbientLightIntensity(float newIntensity, float fadeDuration)
        {
            this.fadeMagnitude = (newIntensity - this.ambientLightIntensity) / fadeDuration;
            this.targetAmbientLightIntensity = newIntensity;
        }

        public void Begin()
        {
            this.objects.Clear();
            this.lights.Clear();
            this.lightBlockers.Clear();
            this.debug.Clear();
            this.menuItems.Clear();
            this.hudItems.Clear();
        }

        public void Render(RendererComponents rendererComponents)
        {
            List<RendererPackage> rendererPackages = rendererComponents.GetRendererPackages();
            foreach (RendererPackage rp in rendererPackages)
            {
                //Add the RenderPackage to the right layer
                switch (rp.layer)
                {
                    case ObjectLayer:
                            this.objects.Add(rp);
                            break;
                    case LightLayer:
                            this.lights.Add(rp);
                            break;
                    case LightBlockerLayer:
                            this.lightBlockers.Add(rp);
                            break;
                    case DebugLayer:
                            this.debug.Add(rp);
                            break;
                    case MenuLayer:
                            this.menuItems.Add(rp);
                            break;
                    case HudLayer:
                            this.hudItems.Add(rp);
                            break;
                        
                }
            }
        }

        public void Render(GameObject gameObject)
        {
            this.Render(gameObject.GetRendererComponents());
        }

        public void Render(GameObjectManager gameObjectManager)
        {         
            List<GameObject> gameObjects = gameObjectManager.GetGameObjects();
            foreach (GameObject go in gameObjects)
            {
                if (go.IsInActiveArea())
                {
                    this.Render(go);
                }
            }
        }

        public void Render(MenuManager menuManager)
        {
            if (MenuManager.showMenu)
            {
                List<GameObject> menuItems = menuManager.GetActiveMenuItems();
                foreach (GameObject go in menuItems)
                {
                    this.Render(go);
                }
            }
        }

        public void Render(HudManager hudManager)
        {
            if (HudManager.showHud)
            {
                List<GameObject> hudItems = hudManager.GetHudItems();
                foreach (GameObject go in hudItems)
                {
                    this.Render(go);
                }
            }
        }

        public void End(ref GraphicsDeviceManager graphics)
        {
            if (DebugManager.visibleDebug)
            {
                this.renderer.RenderDebug(this.debug);
            }
            this.renderer.RenderMenu(this.menuItems);
            this.renderer.RenderHUD(this.hudItems);
            this.renderer.RenderLights(this.lights);
            this.renderer.RenderLevel(this.objects);
         
            this.renderer.Finish();
        }
    }
}
