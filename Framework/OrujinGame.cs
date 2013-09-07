using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orujin.Core.Logic;
using Orujin.Core.Renderer;
using Orujin.Debug;
using Orujin.Pipeline;
using Microsoft.Xna.Framework.Media;

namespace Orujin.Framework
{
    public class OrujinGame
    {
        internal static Orujin orujin {get; private set;}
        public OrujinHud hud { get; internal set; }
        public World world {get; internal set;}
        public string name {get; private set;}
        public bool running = false;
        public static bool ExternalRestart = false;
        public static string musicPath = "";
        public static bool nextLevel = false;
        
        public OrujinGame(string name, Vector2 gravity)
        {
            this.world = new World(gravity);
            this.name = name;
        }

        internal void Initialize(Orujin o)
        {
            orujin = o;
            this.InitializeApplication();
        }

        protected virtual void InitializeApplication()
        {
        }

        public virtual void Load()
        {
        }

        public virtual void PrepareForRestart()
        {
            MenuManager.fadeIn = true;
            this.running = false;
        }

        public virtual void NextLevel(int data)
        {
            nextLevel = true;
            this.PrepareForRestart();
        }

        public static void Restart()
        {
            orujin.Restart();
        }

        public virtual void Start()
        {
            //MenuManager.showMenu = false;
            ExternalRestart = false;
            MenuManager.fadeOut = true;
            this.running = true;
        }

        public virtual void Update(float elapsedTime)
        {
            if (ExternalRestart)
            {
                ExternalRestart = false;
                this.PrepareForRestart();
            }
            this.world.Step(elapsedTime * 0.001f);
        }

        public World GetWorld()
        {
           return this.world;
        }

        /*Attempts to add the GameObject to the game and returns true if it was successful and there are no duplicates*/
        public bool AddObject(GameObject gameObject)
        {
            return orujin.gameObjectManager.Add(gameObject);
        }

        public bool AddObjectAfter(GameObject afterThis, GameObject addThis)
        {
            return orujin.gameObjectManager.AddAfter(afterThis, addThis);
        }

        /*Attempts to remove the GameObject from the game and returns true if it was successful, returns false if the GameObject wasn't found*/
        public bool RemoveObject(GameObject gameObject)
        {
            return orujin.gameObjectManager.Remove(gameObject);
        }
        
        /*Attempts to find a GameObject with the specific name, returns null if no GameObject with the name was found*/
        public GameObject FindObjectWithName(string name)
        {
            return orujin.gameObjectManager.GetByName(name);
        }

        internal CameraManager GetCameraManager()
        {
            return orujin.cameraManager;
        }

        internal DebugManager GetDebugManager()
        {
            return orujin.debugManager;
        }

        internal MenuManager GetMenuManager()
        {
            return orujin.menuManager;
        }

        internal GameObjectManager GetGameObjectManager()
        {
            return orujin.gameObjectManager;
        }

        internal RendererManager GetRendererManager()
        {
            return orujin.rendererManager;
        }

        public void InitializeHud(OrujinHud hud)
        {
            this.hud = hud;
            orujin.InitializeHudManager(this.hud);
        }

        public void InitializeMenus(OrujinMenu startMenu)
        {
            orujin.InitializeMenuManager(startMenu);
        }

        /*Attempts to find one or more GameObjects with a specific tag, returns an empty list if no GameObjects with the tag were found.*/
        public List<GameObject> FindObjectsWithTag(string tag)
        {
            return orujin.gameObjectManager.GetByTag(tag);
        }        

        /*Adds a custom input command to the game.*/
        public void AddInputCommand(string objectName, string methodName, object[] parameters, Keys key, Buttons button, bool pressedOnly)
        {
            orujin.inputManager.AddCommand(objectName, methodName, parameters, key, button, pressedOnly);
        }

        public void AddEventCondition(GameEventCondition condition)
        {
            orujin.conditionManager.AddCondition(condition);
        }

        public void RemoveEventCondition(int id)
        {
            orujin.conditionManager.RemoveCondition(id);
        }

        public static Texture2D GetTexture2DByName(String name)
        {
            return orujin.GetTexture2DByName(name);
        }

        public static Texture2D GetTexture2DByColor(Color color)
        {
            return orujin.GetTexture2DByColor(color);
        }

        public static Effect GetEffect(String name)
        {
            return orujin.GetEffect(name);
        }

        public static SpriteAnimation GetSpriteAnimationByName(String name)
        {
            return orujin.GetSpriteAnimationByName(name);
        }

        public static ModularAnimation GetModularAnimationByName(String name)
        {
            return orujin.GetModularAnimationByName(name);
        }

        public static void PlaySong(String path, bool isRepeating)
        {
            orujin.PlayMusic(path, isRepeating);
        }

        public static void PlaySoundEffect(String path)
        {
            orujin.PlaySoundEffect(path);
        }

        public static void PauseMusic()
        {
            orujin.PauseMusic();
        }

        public static void ResumeMusic()
        {
            orujin.ResumeMusic();
        }

        public static void SetMusicVolume(float volume)
        {
            orujin.SetMusicVolume(volume);
        }

        protected void LoadLevel(string fileName, ObjectProcessor op)
        {
            orujin.LoadLevel(fileName, op);
        }

        public void RemotelyTriggerEvent(int i)
        {
            orujin.conditionManager.RemotelyFulfillCondition(i);
        }

        public virtual void WhenScaled()
        {
        }

        public void PauseLogic(int data)
        {
            orujin.updateLogic = false;
        }

        public void ResumeLogic(int data)
        {
            orujin.updateLogic = true;
        }

        public void SetAmbientLightIntensity(float intensity)
        {
            orujin.SetAmbientLightIntensity(intensity);
        }

        public virtual object GetUserData()
        {
            return null;
        }

        public virtual string GetProperties()
        {
            return "";
        }
    }
}
