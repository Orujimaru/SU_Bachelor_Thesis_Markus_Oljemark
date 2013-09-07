using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Orujin.Core.GameHelp
{
    public static class OrujinMathHelper
    {
        private static Random random = new Random();

        public static bool Intersects(Vector2 point, Rectangle rectangle)
        {
            if (point.X < rectangle.X || point.X > rectangle.X + rectangle.Width)
            {
                return false;
            }
            if (point.Y < rectangle.Y || point.Y > rectangle.Y + rectangle.Height)
            {
                return false;
            }
            return true;
        }

        public static bool Intersects(Rectangle a, Rectangle b)
        {
            throw new NotImplementedException();
        }

        public static Vector2 GetDirectionFromAToB(Vector2 a, Vector2 b)
        {
            Vector2 direction = new Vector2(b.X - a.X, b.Y - a.Y);
            direction.Normalize();
            return direction;
        }

        public static int GetNextRandom(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}
