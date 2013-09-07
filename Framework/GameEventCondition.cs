using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orujin.Framework
{
    public class GameEventCondition
    {
        #region DEFINED IDS
        public const int RightCameraBorderId = 1001;
        public const int LeftCameraBorderId = 1002;
        public const int BottomCameraBorderId = 1003;
        public const int UnlockCameraHorizontallyId = 1004;
        public const int LockCameraHorizontallyId = 1005;
        public const int UnlockCameraVerticallyId = 1006;
        public const int LockCameraVerticallyId = 1007;
        #endregion DEFINED IDS
        public static int counter { get; private set; }
        public int id { get; private set; }
        public bool fulfilled { get; private set; }
        public GameEvent ev { get; private set; }

        public GameEventCondition(int id, GameEvent ev)
        {
            counter++;
            this.id = id;
            this.ev = ev;
            this.fulfilled = false;
        }

        public void Update(float elapsedTime)
        {
            if (this.fulfilled)
            {
                ev.Update(elapsedTime);
                if (ev.finished)
                {
                    this.fulfilled = false;
                }
            }
        }

        public void Fulfill()
        {
            this.fulfilled = true;
            ev.Start();
        }

    }
}
