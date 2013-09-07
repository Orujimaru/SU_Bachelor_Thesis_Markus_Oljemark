using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Orujin.Framework;
using Orujin.Core.Logic;

namespace Orujin.Core.Input
{
    public class InputManager
    {
        private List<InputCommand> commands;
        private const float ThumbstickThreshold = 0.5f;
        private MouseManager mouse;

        public InputManager()
        {
            this.commands = new List<InputCommand>();
            this.mouse = new MouseManager();
        }

        public void AddCommand(string objectName, string methodName, object[] parameters, Keys key, Buttons button, bool pressedOnly)
        {
            InputCommand inputCommand = new InputCommand();
            inputCommand.objectName = objectName;
            inputCommand.methodName = methodName;
            inputCommand.parameters = parameters;
            inputCommand.key = key;
            inputCommand.button = button;
            inputCommand.isDown = false;
            inputCommand.pressed = false;
            inputCommand.magnitude = 0;
            inputCommand.pressedOnly = pressedOnly;

            if (button.Equals(Buttons.LeftThumbstickDown) || button.Equals(Buttons.LeftThumbstickUp) || button.Equals(Buttons.LeftThumbstickLeft) || button.Equals(Buttons.LeftThumbstickRight) || 
                button.Equals(Buttons.RightThumbstickDown) || button.Equals(Buttons.RightThumbstickUp) || button.Equals(Buttons.RightThumbstickLeft) || button.Equals(Buttons.RightThumbstickRight))
            {
                inputCommand.thumbstick = true;
            }
            else
            {
                inputCommand.thumbstick = false;
            }
            this.commands.Add(inputCommand);
        }

        public List<InputCommand> Update(float elapsedTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            this.mouse.Update(elapsedTime);

            for (int i = 0; i < this.commands.Count(); i++)
            {
                InputCommand inputCommand = this.commands[i];
                if (keyboard.IsKeyDown(inputCommand.key) || (gamePad.IsButtonDown(inputCommand.button) && !inputCommand.thumbstick) ||
                    this.ThumbsticksMatch(ref inputCommand, gamePad))
                {
                    this.ActivateCommand(ref inputCommand);
                }
                else
                {
                    this.DeactivateCommand(ref inputCommand);
                }
                this.commands[i] = inputCommand;
            }
            this.CheckInput();
            return this.commands;
        }

        private void CheckInput()
        {
            foreach (InputCommand ic in this.commands)
            {
                if (ic.pressed || (ic.isDown && !ic.pressedOnly))
                {
                    if (ic.objectName.Equals("Camera"))
                    {
                        MethodInfo method = GameManager.game.GetCameraManager().GetType().GetMethod(ic.methodName);
                        method.Invoke(GameManager.game.GetCameraManager(), ic.parameters);
                    }
                    else if (ic.objectName.Equals("Renderer"))
                    {
                        MethodInfo method = GameManager.game.GetRendererManager().GetType().GetMethod(ic.methodName);
                        method.Invoke(GameManager.game.GetRendererManager(), ic.parameters);
                    }
                    else if (ic.objectName.Equals("Game"))
                    {
                        MethodInfo method = GameManager.game.GetType().GetMethod(ic.methodName);
                        method.Invoke(GameManager.game, ic.parameters);
                    }
                    else if (ic.objectName.Equals("Debug"))
                    {
                        MethodInfo method = GameManager.game.GetDebugManager().GetType().GetMethod(ic.methodName);
                        method.Invoke(GameManager.game.GetDebugManager(), ic.parameters);
                    }
                    else if (ic.objectName.Equals("Menu"))
                    {
                        MethodInfo method = GameManager.game.GetMenuManager().GetType().GetMethod(ic.methodName);
                        method.Invoke(GameManager.game.GetMenuManager(), ic.parameters);
                    }
                    else if (ic.objectName.Equals("Hud"))
                    {
                        HudManager.hud.ActivateItem((string)ic.parameters[0]);
                    }
                    else
                    {
                        GameManager.game.GetGameObjectManager().CheckForInput(ic);
                    }
                }
            }
        }

        private void ActivateCommand(ref InputCommand inputCommand)
        {
            if (!inputCommand.isDown)
            {
                inputCommand.pressed = true;
            }
            else
            {
                inputCommand.pressed = false;
            }
            inputCommand.isDown = true;

        }

        private void DeactivateCommand(ref InputCommand inputCommand)
        {
            inputCommand.pressed = false;
            inputCommand.isDown = false;
        }

        private bool ThumbsticksMatch(ref InputCommand inputCommand, GamePadState gamePad)
        {
            switch(inputCommand.button)
            {
                case Buttons.LeftThumbstickUp:
                {
                    if (gamePad.ThumbSticks.Left.Y > ThumbstickThreshold)
                    {
                        inputCommand.magnitude = (Math.Abs(gamePad.ThumbSticks.Left.Y) - ThumbstickThreshold) * 2;
                        return true;
                    }
                    break;
                }
                
                case Buttons.LeftThumbstickLeft:
                {
                    if (gamePad.ThumbSticks.Left.X < -ThumbstickThreshold)
                    {
                        inputCommand.magnitude = (Math.Abs(gamePad.ThumbSticks.Left.X) - ThumbstickThreshold) * 2;
                        return true;
                    }
                    break;
                }
                
                case Buttons.LeftThumbstickDown:
                {
                    if (gamePad.ThumbSticks.Left.Y < -ThumbstickThreshold)
                    {
                        inputCommand.magnitude = (Math.Abs(gamePad.ThumbSticks.Left.Y) - ThumbstickThreshold) * 2;
                        return true;
                    }
                    break;
                }
                
                case Buttons.LeftThumbstickRight:
                {
                    if (gamePad.ThumbSticks.Left.X > ThumbstickThreshold)
                    {
                        inputCommand.magnitude = (Math.Abs(gamePad.ThumbSticks.Left.X) - ThumbstickThreshold) * 2;
                        return true;
                    }
                    break;
                }
                case Buttons.RightThumbstickUp:
                {
                    if (gamePad.ThumbSticks.Right.Y > ThumbstickThreshold)
                    {
                        inputCommand.magnitude = (Math.Abs(gamePad.ThumbSticks.Right.Y) - ThumbstickThreshold) * 2;
                        return true;
                    }
                    break;
                }
                
                case Buttons.RightThumbstickLeft:
                {
                    if (gamePad.ThumbSticks.Right.X < -ThumbstickThreshold)
                    {
                        inputCommand.magnitude = (Math.Abs(gamePad.ThumbSticks.Right.X) - ThumbstickThreshold) * 2;
                        return true;
                    }
                    break;
                }
                
                case Buttons.RightThumbstickDown:
                {
                    if (gamePad.ThumbSticks.Right.Y < -ThumbstickThreshold)
                    {
                        inputCommand.magnitude = (Math.Abs(gamePad.ThumbSticks.Right.Y) - ThumbstickThreshold) * 2;
                        return true;
                    }
                    break;
                }
                
                case Buttons.RightThumbstickRight:
                {
                    if (gamePad.ThumbSticks.Right.X > ThumbstickThreshold)
                    {
                        inputCommand.magnitude = (Math.Abs(gamePad.ThumbSticks.Right.X) - ThumbstickThreshold) * 2;
                        return true;
                    }
                    break;
                }
            }
            inputCommand.magnitude = 1;
            return false;
        }
    }

    public struct InputCommand
    {
        public string objectName;
        public string methodName;
        public object[] parameters;
        public Keys key;
        public Buttons button;
        public bool thumbstick;
        public bool isDown;
        public bool pressed;
        public bool pressedOnly;
        public float magnitude;
    }
}
