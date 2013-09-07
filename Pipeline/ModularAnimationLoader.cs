using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Orujin.Core.Renderer;

namespace Orujin.Pipeline
{
    internal class ModularAnimationLoader
    {
        public static ModularAnimation Load(string animationName)
        {
            String line;
            int numberOfFrames;

            try
            {
                using (StreamReader sr = new StreamReader("Content/Animations/ModularAnimations/" + animationName + ".txt"))
                {
                    line = sr.ReadToEnd();
                }
                string[] frames = line.Split('|');
                numberOfFrames = frames.Count() - 1;

                if (numberOfFrames <= 0)
                {
                    throw new Exception();
                }

                List<ModularAnimation.Frame> modularFrames = new List<ModularAnimation.Frame>();

                //Go through each frame in the array and add it to the list of frames
                for (int s = 0; s < numberOfFrames; s++)
                {
                    string[] frameComponents = frames[s].Split(',');

                    float duration;
                    float rotation;
                    float scaleX;
                    float scaleY;
                    float positionX;
                    float positionY;

                    float.TryParse(frameComponents[0], NumberStyles.Any, CultureInfo.InvariantCulture, out duration);
                    float.TryParse(frameComponents[1], NumberStyles.Any, CultureInfo.InvariantCulture, out rotation);
                    float.TryParse(frameComponents[2], NumberStyles.Any, CultureInfo.InvariantCulture, out scaleX);
                    float.TryParse(frameComponents[3], NumberStyles.Any, CultureInfo.InvariantCulture, out scaleY);
                    float.TryParse(frameComponents[4], NumberStyles.Any, CultureInfo.InvariantCulture, out positionX);
                    float.TryParse(frameComponents[5], NumberStyles.Any, CultureInfo.InvariantCulture, out positionY);

                    ModularAnimation.Frame tempFrame = new ModularAnimation.Frame();
                    tempFrame.duration = duration;
                    tempFrame.rotation = rotation;
                    tempFrame.scale = new Vector2(scaleX, scaleY);
                    tempFrame.positionOffset = new Vector2(positionX, positionY);

                    modularFrames.Add(tempFrame);
                }

                ModularAnimation modularAnimation = new ModularAnimation(modularFrames);
                return modularAnimation;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
