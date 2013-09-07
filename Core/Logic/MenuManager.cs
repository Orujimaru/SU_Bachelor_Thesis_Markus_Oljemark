using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orujin.Core.Input;
using Orujin.Core.Renderer;

namespace Orujin.Framework
{
    public class MenuManager
    {
#region AlignValues
        public const int LeftAlign = 1;
        public const int RightAlign = 2;
        public const int CenterAlign = 3;
        public const int TopAlign = 4;
        public const int BottomAlign = 5;

        public const int TopLeft = 1;
        public const int TopCenter = 2;
        public const int TopRight = 3;
        public const int CenterLeft = 4;
        public const int CenterCenter = 5;
        public const int CenterRight = 6;
        public const int BottomLeft = 7;
        public const int BottomCenter = 8;
        public const int BottomRight = 9;
#endregion AlignValues

        public List<OrujinMenu> menus { get; private set; }
        public static OrujinMenu activeMenu = null;
        public static bool showMenu = true;
        public static bool fadeIn = false;
        public static bool fadeOut = false;

        public MenuManager(OrujinMenu orujinMenu)
        {
            this.menus = new List<OrujinMenu>();
            this.menus.Add(orujinMenu);
            activeMenu = orujinMenu;
        }

        public void AddMenu(OrujinMenu orujinMenu)
        {
            menus.Add(orujinMenu);
        }

        public void SetActiveMenu(string name)
        {
            foreach (OrujinMenu menu in this.menus)
            {
                if (menu.name.Equals(name))
                {
                    activeMenu = menu;
                }
            }
        }

        public void Update(float elapsedTime)
        {
            if (fadeIn)
            {
                activeMenu.Reset();
                activeMenu.FadeIn(2000);
                fadeIn = false;
            }
            if (fadeOut)
            {
                activeMenu.FadeOut(1000);
                fadeOut = false;
            }
            activeMenu.Update(elapsedTime);
        }

        public void ToggleMenu(string name)
        {
            if (showMenu)
            {
                showMenu = false;
                return;
            }
            else
            {
                showMenu = true;
                if (activeMenu.name.Equals(name))
                {
                    return;
                }
                else
                {
                    this.SetActiveMenu(name);
                }
            }
        }

        public List<GameObject> GetActiveMenuItems()
        {
            return activeMenu.GetGameObjects();
        }

        public static Vector2 GetPosition(Texture2D texture, int location, int horizontalAlignment, int verticalAlignment)
        {
            float screenWidth = Renderer.width;
            float screenHeight = Renderer.height;

            float textureWidth = texture.Bounds.X;
            float textureHeight = texture.Bounds.Y;

            float x = 0;
            float y = 0;

            float xOffset = 0;
            float yOffset = 0;

            switch(horizontalAlignment)
            {
                case LeftAlign:
                    xOffset = -textureWidth/2;
                    break;
                case RightAlign:
                    xOffset = textureWidth/2;
                    break;
            }
            switch (verticalAlignment)
            {
                case TopAlign:
                    yOffset = -textureHeight / 2;
                    break;
                case BottomAlign:
                    yOffset = textureHeight / 2;
                    break;
            }

            switch (location)
            {
                case TopLeft:
                    x = (screenWidth * 0.15f) + xOffset;
                    y = (screenHeight * 0.15f) + yOffset;
                    break;

                case TopCenter:
                    x = (screenWidth * 0.5f) + xOffset;
                    y = (screenHeight * 0.15f) + yOffset;
                    break;

                case TopRight:
                    x = (screenWidth * 0.85f) + xOffset;
                    y = (screenHeight * 0.15f) + yOffset;
                    break;

                case CenterLeft:
                    x = (screenWidth * 0.15f) + xOffset;
                    y = (screenHeight * 0.5f) + yOffset;
                    break;

                case CenterCenter:
                    x = (screenWidth * 0.5f) + xOffset;
                    y = (screenHeight * 0.5f) + yOffset;
                    break;

                case CenterRight:
                    x = (screenWidth * 0.85f) + xOffset;
                    y = (screenHeight * 0.5f) + yOffset;
                    break;

                case BottomLeft:
                    x = (screenWidth * 0.15f) + xOffset;
                    y = (screenHeight * 0.85f) + yOffset;
                    break;

                case BottomCenter:
                    x = (screenWidth * 0.5f) + xOffset;
                    y = (screenHeight * 0.85f) + yOffset;
                    break;

                case BottomRight:
                    x = (screenWidth * 0.85f) + xOffset;
                    y = (screenHeight * 0.85f) + yOffset;
                    break;
            }
            return new Vector2(x, y);
        }

    }
}
