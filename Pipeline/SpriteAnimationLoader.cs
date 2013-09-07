using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Orujin.Core.Renderer;

namespace Orujin.Pipeline
{
    internal class SpriteAnimationLoader
    {
        public static SpriteAnimation Load(string animationName)
        {
            String line;
            int numberOfFrames;
            try
            {
                using (StreamReader sr = new StreamReader("Content/Animations/SpriteAnimations/"  + animationName + ".txt"))
                {
                    line = sr.ReadToEnd();
                }
                string[] frames = line.Split('|');
                numberOfFrames = frames.Count() - 1;

                if (numberOfFrames <= 0)
                {
                    throw new Exception();
                }
                
                List<SpriteAnimation.Frame> spriteFrames = new List<SpriteAnimation.Frame>();

                //Go through each frame in the array and add it to the list of frames
                for (int s = 0; s < numberOfFrames; s++)
                {
                    string[] frameComponents = frames[s].Split(','); 
                   
                    float duration;              
                    int x;
                    int y;
                    int width;
                    int height;

                    float.TryParse(frameComponents[0], NumberStyles.Any, CultureInfo.InvariantCulture, out duration); 
                    Int32.TryParse(frameComponents[1], out x);
                    Int32.TryParse(frameComponents[2], out y);
                    Int32.TryParse(frameComponents[3], out width);
                    Int32.TryParse(frameComponents[4], out height);

                    SpriteAnimation.Frame tempFrame = new SpriteAnimation.Frame();
                    tempFrame.duration = duration;
                    tempFrame.source = new Rectangle(x, y, width, height);
                    spriteFrames.Add(tempFrame);
                }
                SpriteAnimation spriteAnimation = new SpriteAnimation(spriteFrames);
                return spriteAnimation;
            }
            catch (Exception e)
            {
                return null;
            }                
        }
    }
}
