using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orujin.Framework;

namespace Orujin.Pipeline
{
    public struct ObjectInformation
    {
        public String texturePath;
        public String textureName;
        public String name;
        public String layer;
        public Vector2 position;
        public Vector2 origin;
        public Vector2 scrollSpeed;
        public List<String> customProperties;
        public Texture2D texture;
        public Vector2 scale;
        public float rotation;
        public bool flipH;
        public bool flipV;
        public float width;
        public float height;
    }
    public abstract class ObjectProcessor
    {
        public abstract void ProcessTextureObject(ObjectInformation oi);
        public abstract void ProcessPrimitiveObject(ObjectInformation oi);
    }
}
