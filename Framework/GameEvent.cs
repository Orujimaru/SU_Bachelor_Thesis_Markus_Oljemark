using Orujin.Core.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orujin.Framework
{
    public class GameEventCommand
    {
        public GameEventCommand(String objectName, String objectTag, String methodName, object[] parameters, float delay)
        {
            this.objectName = objectName;
            this.objectTag = objectTag;
            this.methodName = methodName;
            this.parameters = parameters;
            this.delay = delay;
            this.initialDelay = delay;
        }
        public string objectName;
        public string objectTag = null;
        public string methodName;
        public object[] parameters;
        public float delay;
        public float initialDelay;
    }

    public class GameEvent
    {
        public bool finished { get; private set; }
        private List<GameEventCommand> eventCommands;
        private int index = 0;

        public GameEvent(List<GameEventCommand> eventCommands)
        {
            this.eventCommands = eventCommands;
            this.finished = false;
        }

        public GameEvent(GameEventCommand ev)
        {
            this.eventCommands = new List<GameEventCommand>();
            this.eventCommands.Add(ev);
            this.finished = false;
        }

        public void Update(float elapsedTime)
        {
            if (this.index == this.eventCommands.Count())
            {
                this.finished = true;
            }
            else if (!this.finished)
            {
                this.eventCommands[this.index].delay -= elapsedTime;
                if (this.eventCommands[this.index].delay <= 0)
                {
                    this.InvokeCommand(this.eventCommands[this.index]);
                }
            }
        }

        public void Start()
        {
            this.finished = false;
            this.index = 0;
            foreach (GameEventCommand gev in this.eventCommands)
            {
                gev.delay = gev.initialDelay;
            }
        }
        private void InvokeCommand(GameEventCommand eventCommand)
        {
            this.index++;
            if (eventCommand.objectTag != null)
            {
                foreach (GameObject gameObject in GameManager.game.FindObjectsWithTag(eventCommand.objectTag))
                {
                    MethodInfo method = gameObject.GetType().GetMethod(eventCommand.methodName);
                    method.Invoke(gameObject, eventCommand.parameters);
                }
            }
            else
            {
                if (eventCommand.objectName.Equals("Camera"))
                {
                    MethodInfo method = GameManager.game.GetCameraManager().GetType().GetMethod(eventCommand.methodName);
                    method.Invoke(GameManager.game.GetCameraManager(), eventCommand.parameters);
                }
                else if (eventCommand.objectName.Equals("Game"))
                {
                    MethodInfo method = GameManager.game.GetType().GetMethod(eventCommand.methodName);
                    method.Invoke(GameManager.game, eventCommand.parameters);
                }
                else if (eventCommand.objectName.Equals("Renderer"))
                {
                    MethodInfo method = GameManager.game.GetRendererManager().GetType().GetMethod(eventCommand.methodName);
                    method.Invoke(GameManager.game.GetRendererManager(), eventCommand.parameters);
                }
                else if (eventCommand.objectName.Equals("Debug"))
                {
                    MethodInfo method = GameManager.game.GetDebugManager().GetType().GetMethod(eventCommand.methodName);
                    method.Invoke(GameManager.game.GetDebugManager(), eventCommand.parameters);
                }
                else if (eventCommand.objectName.Equals("Menu"))
                {
                    MethodInfo method = GameManager.game.GetMenuManager().GetType().GetMethod(eventCommand.methodName);
                    method.Invoke(GameManager.game.GetMenuManager(), eventCommand.parameters);
                }
                else if (eventCommand.objectName.Equals("Hud"))
                {
                    HudManager.hud.ActivateItem((string)eventCommand.parameters[0]);
                }
                else
                {
                    GameObject gameObject = GameManager.game.FindObjectWithName(eventCommand.objectName);
                    MethodInfo method = gameObject.GetType().GetMethod(eventCommand.methodName);
                    method.Invoke(gameObject, eventCommand.parameters);
                }

            }
            
        }
    }
}
