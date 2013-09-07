using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orujin.Framework;

namespace Orujin.Core.Logic
{
    public class HudManager
    {
        public static OrujinHud hud { get; private set; }
        public static bool showHud = true;

        public HudManager(OrujinHud nhud)
        {
            hud = nhud;
        }

        public void Update(float elapsedTime)
        {
            hud.Update(elapsedTime);
        }

        public List<GameObject> GetHudItems()
        {
            return hud.GetGameObjects();
        }
    }
}
