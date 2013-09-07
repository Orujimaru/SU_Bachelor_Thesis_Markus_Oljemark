using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Orujin.Core.Renderer;
using Orujin.Framework;

namespace Orujin.Core.Physics
{
    public class PixelCollisionManager
    {
        public static bool Intersects(Sprite spriteA, Sprite spriteB)
        {
            if(spriteA.PrepareRendererPackage().destination.Intersects(spriteB.PrepareRendererPackage().destination))
            {
                return IntersectsPixels(spriteA, spriteB);
            }
            return false;
        }

        public static bool IntersectsPixels(Sprite spriteA, Sprite spriteB)
        {           
            Rectangle destinationA = spriteA.PrepareRendererPackage().destination;
            Rectangle destinationB = spriteB.PrepareRendererPackage().destination;

            // Find the bounds of the rectangle intersection
            int top = Math.Max(destinationA.Top, destinationB.Top);
            int bottom = Math.Min(destinationA.Bottom, destinationB.Bottom);
            int left = Math.Max(destinationA.Left, destinationB.Left);
            int right = Math.Min(destinationA.Right, destinationB.Right);

            Color[] dataA = spriteA.GetColorData();
            Color[] dataB = spriteB.GetColorData();

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - destinationA.Left) +
                                         (y - destinationA.Top) * destinationA.Width];
                    Color colorB = dataB[(x - destinationB.Left) +
                                         (y - destinationB.Top) * destinationB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A > 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }
    }
}
