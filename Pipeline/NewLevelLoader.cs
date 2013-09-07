using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Orujin.Framework;

namespace Orujin.Pipeline
{
    public partial class NewLevelLoader
    {
        private static ObjectProcessor objectProcessor;
        private static int testCounter = 0;


        public NewLevelLoader()
        {
        }

        public static void FromFile(string filename, ContentManager cm, ObjectProcessor newObjectProcessor)
        {
            objectProcessor = newObjectProcessor;
            StreamReader stream = new StreamReader(filename);
            String str = stream.ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(str);

            XmlNode level = doc.FirstChild.NextSibling;
            XmlNode layers = level.FirstChild.NextSibling;

            foreach (XmlNode layer in layers.ChildNodes)
            {
                String layerName = layer.FirstChild.FirstChild.InnerText;
                Vector2 scrollSpeed = ExtractVector2(layer.FirstChild.LastChild);
                
                XmlNode items = layer.FirstChild.NextSibling;
                foreach (XmlNode item in items.ChildNodes)
                {
                    if (item.FirstChild.Name.Equals("RectangleItemProperties"))
                    {
                        ProcessRectangleItem(layerName, scrollSpeed, item.FirstChild);
                    }
                    else if (item.FirstChild.Name.Equals("TextureItemProperties"))
                    {
                        ProcessTextureItem(layerName, scrollSpeed, item.FirstChild, cm);
                    }
                }
            }
        }

        private static void ProcessRectangleItem(string layer, Vector2 scrollSpeed, XmlNode properties)
        {
            ObjectInformation oi = new ObjectInformation();
            oi.scrollSpeed = scrollSpeed;
            oi.layer = layer;
            oi.name = "TEMP";

            foreach (XmlNode child in properties.ChildNodes)
            {
                switch (child.Name)
                {
                    case "Name":
                        oi.name = child.InnerText;
                        break;

                    case "Position":
                        oi.position = ExtractVector2(child);
                        break;

                    case "CustomProperties":
                        oi.customProperties = ExtractCustomProperties(child);
                        break;

                    case "Width":
                        oi.width = ExtractFloat(child);
                        break;

                    case "Height":
                        oi.height = ExtractFloat(child);
                        break;

                    case "Rotation":
                        oi.rotation = ExtractFloat(child);
                        break;
                }
            }

            //Adjust the position since Gleed handles textures' and primitives' positions differently.
            oi.position.X += oi.width / 2;
            oi.position.Y += oi.height / 2;

            objectProcessor.ProcessPrimitiveObject(oi);
        }

        private static void ProcessTextureItem(string layer, Vector2 scrollSpeed, XmlNode properties, ContentManager cm)
        {
            ObjectInformation oi = new ObjectInformation();
            oi.scrollSpeed = scrollSpeed;
            oi.layer = layer;
            oi.name = "TEMP";

            foreach (XmlNode child in properties.ChildNodes)
            {
                switch (child.Name)
                {
                    case "Name":
                        oi.name = child.InnerText;
                        break;

                    case "Position":
                        oi.position = ExtractVector2(child);
                        break;

                    case "CustomProperties":
                        oi.customProperties = ExtractCustomProperties(child);
                        break;

                    case "Rotation":
                        oi.rotation = ExtractFloat(child);
                        break;

                    case "Scale":
                        oi.scale = ExtractVector2(child); ;
                        break;

                    case "FlipHorizontally":
                        oi.flipH = ExtractBool(child);
                        break;

                    case "FlipVertically":
                        oi.flipV = ExtractBool(child);
                        break;

                    case "TexturePathRelativeToContentRoot":

                        String[] splitString = child.FirstChild.Value.Split('\\');
                        
                        try
                        {
                            oi.texture = cm.Load<Texture2D>(child.FirstChild.Value);
                        }
                        //This causes major performance issues at the moment
                        catch (ContentLoadException e)
                        {
                        }

                        
                        break;
                }
            }
            objectProcessor.ProcessTextureObject(oi);
        }

        private static Vector2 ExtractVector2(XmlNode target)
        {
            String sX = target.FirstChild.FirstChild.Value;
            String sY = target.LastChild.FirstChild.Value;

            sX = sX.Replace('.', ',');
            sY = sY.Replace('.', ',');

            return new Vector2(float.Parse(sX), float.Parse(sY));
        }

        private static float ExtractFloat(XmlNode target)
        {
            String sFloat = target.FirstChild.InnerText;
            sFloat = sFloat.Replace('.', ',');
            float f = float.Parse(sFloat);
            return f;
        }

        private static List<String> ExtractCustomProperties(XmlNode target)
        {
            List<String> customProperties = new List<String>();
            if (target.HasChildNodes && target.FirstChild.HasChildNodes)
            {
                foreach (XmlNode property in target.ChildNodes)
                {
                        customProperties.Add(property.InnerText);
                }
            }
            return customProperties;
        }

        private static bool ExtractBool(XmlNode target)
        {
            String sFlipHorizontally = target.FirstChild.Value;
            bool flipHorizontally = false;
            if (sFlipHorizontally.Equals("True", StringComparison.OrdinalIgnoreCase))
            {
                flipHorizontally = true;
            }
            return flipHorizontally;
        }
    }
}
