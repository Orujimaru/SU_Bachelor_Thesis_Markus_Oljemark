#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Orujin.Core.Renderer;
using Orujin.Framework;
using Orujin.Core.Logic;
using Orujin.Core.Input;
using System.Reflection;
using Orujin.Core.GameHelp;
using Orujin.Debug;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Orujin.Pipeline;
using Orujin.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Orujin
{
    public class Orujin : Game
    {
        private GraphicsDeviceManager graphics;
        internal RendererManager rendererManager { get; private set; }
        internal GameObjectManager gameObjectManager { get; private set; }
        internal MenuManager menuManager { get; private set; }
        internal HudManager hudManager { get; private set; }
        internal GameEventConditionManager conditionManager { get; private set; }
        internal InputManager inputManager { get; private set; }
        internal CameraManager cameraManager { get; private set; }
        internal DebugManager debugManager { get; private set; }
        private SoundEffectInstance music = null;
        private List<SoundEffect> soundEffects = new List<SoundEffect>();
        internal bool updateLogic = true;

        public Orujin()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //this.IsFixedTimeStep = false;
            this.rendererManager = new RendererManager(ref this.graphics, Content.Load<SpriteFont>("Fonts/Font"), 1280, 720);
            this.rendererManager.SetAmbientLightIntensity(1f);
            this.gameObjectManager = new GameObjectManager();
            this.conditionManager = new GameEventConditionManager();
            this.inputManager = new InputManager();
            this.cameraManager = new CameraManager(1280, 720);
            this.debugManager = new DebugManager();

            this.debugManager.InitiateFarseerDebugView(this.graphics.GraphicsDevice, this.Content, GameManager.game.world);
            GameManager.game.Initialize(this);

            base.Initialize();
        }

        public void Restart()
        {
            Content.Unload();
            this.cameraManager = new CameraManager(1280, 720);
            this.gameObjectManager.Clear();
            this.conditionManager = new GameEventConditionManager();
            this.inputManager = new InputManager();
            this.debugManager = new DebugManager();
            this.debugManager.InitiateFarseerDebugView(this.graphics.GraphicsDevice, this.Content, GameManager.game.world);
            GameManager.game.Initialize(this);
        }

        public void InitializeMenuManager(OrujinMenu menu)
        {
            this.menuManager = new MenuManager(menu);
        }

        public void InitializeHudManager(OrujinHud hud)
        {
            this.hudManager = new HudManager(hud);
        }

        protected override void LoadContent()
        {    
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            long elapsedTime = gameTime.ElapsedGameTime.Milliseconds;

            this.cameraManager.Update(elapsedTime);
            this.rendererManager.Update(elapsedTime);

            if (this.updateLogic)
            {
                List<InputCommand> input = this.inputManager.Update(elapsedTime);
            }

            this.conditionManager.Update(elapsedTime, this.gameObjectManager.GetGameObjects());

            if (this.updateLogic && GameManager.game.running)
            {
                this.gameObjectManager.Update(elapsedTime);
                GameManager.game.Update(elapsedTime);
            }

            if (this.menuManager != null)
            {
                this.menuManager.Update(elapsedTime);
            }
            if (this.hudManager != null)
            {
                this.hudManager.Update(elapsedTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.rendererManager.Begin();

            this.rendererManager.Render(this.gameObjectManager);

            if (this.hudManager != null)
            {
                this.rendererManager.Render(this.hudManager);
            }

            if (this.menuManager != null)
            {
                this.rendererManager.Render(this.menuManager);
            }

            this.rendererManager.End(ref this.graphics);

            this.debugManager.RenderFarseerDebugView(this.graphics);

            base.Draw(gameTime);
        }

        public Texture2D GetTexture2DByName(String name)
        {
            return Content.Load<Texture2D>(name);
        }

        public Texture2D GetTexture2DByColor(Color color)
        {
            Texture2D tempTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] c = new Color[1];
            c[0] = color;
            tempTexture.SetData(c);
            return tempTexture;
        }

        public void PlayMusic(String path, bool isRepeating)
        {
            //MediaPlayer.IsRepeating = isRepeating;
            //Song s = Content.Load<Song>(path);
            //MediaPlayer.Play(s);

            if (this.music != null)
            {
                this.music.Dispose();
            }

            SoundEffect soundEffect = null;

            foreach (SoundEffect se in this.soundEffects)
            {
                if (path.Equals(se.Name))
                {
                    soundEffect = se;
                    break;
                }
            }
            if (soundEffect == null)
            {
                soundEffect = Content.Load<SoundEffect>(path);
                soundEffect.Name = path;
                this.soundEffects.Add(soundEffect);
            }

            
            this.music = soundEffect.CreateInstance();
            this.music.Volume = 0.9f;
            this.music.Play();
            this.music.IsLooped = true;
        }


        public void PlaySoundEffect(String path)
        {
            SoundEffect soundEffect = null;

            foreach (SoundEffect se in this.soundEffects)
            {
                if (path.Equals(se.Name))
                {
                    soundEffect = se;
                    break;
                }
            }
            if (soundEffect == null)
            {
                soundEffect = Content.Load<SoundEffect>(path);
                soundEffect.Name = path;
                this.soundEffects.Add(soundEffect);
            }
            soundEffect.Play();
        }

        public void PauseMusic()
        {
            if (this.music != null)
            {
                this.music.Volume = 0.2f;
            }
        }

        public void ResumeMusic()
        {
            if (this.music != null)
            {
                this.music.Volume = 0.9f;
            }
        }

        public void SetMusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }

        public SpriteAnimation GetSpriteAnimationByName(String name)
        {
            return SpriteAnimationLoader.Load(name);
        }

        public ModularAnimation GetModularAnimationByName(String name)
        {
            return ModularAnimationLoader.Load(name);
        }

        public Effect GetEffect(String name)
        {
            return Content.Load<Effect>(name);
        }

        public void LoadLevel(String fileName, ObjectProcessor op)
        {
            LevelLoader.FromFile(fileName, this.Content, op);
        }

        public void SetAmbientLightIntensity(float intensity)
        {
            this.rendererManager.SetAmbientLightIntensity(intensity);
        }
    }
}
