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
    public partial class LevelLoader
    {
        private static ObjectProcessor objectProcessor;
        private static int testCounter = 0;
        public LevelLoader()
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
            XmlNode layers = level.FirstChild;

            foreach (XmlNode layer in layers.ChildNodes)
            {
                Vector2 scrollSpeed = ExtractVector2(layer.LastChild.PreviousSibling);
                String layerName = GetLayerName(layer);

                XmlNode items = layer.FirstChild;
                foreach (XmlNode item in items.ChildNodes)
                {
                    if (item.OuterXml.StartsWith("<Item xsi:type=\"TextureItem\""))
                    {
                        ParseTextureItem(item, scrollSpeed, layerName, cm);
                    }
                    else if (item.OuterXml.StartsWith("<Item xsi:type=\"RectangleItem\""))
                    {
                        ParseRectangleItem(item, scrollSpeed, layerName);
                    }
                }
            }           
        }

        private static void ParseTextureItem(XmlNode textureItem, Vector2 scrollSpeed, String layerName, ContentManager cm)
        {
            ObjectInformation oi = new ObjectInformation();
            oi.scrollSpeed = scrollSpeed;
            oi.layer = layerName;
            oi.name = "TEMP";
            
            foreach(XmlNode child in textureItem.ChildNodes)
            {
                switch (child.Name)
                {
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
                        oi.scale = ExtractVector2(child);;
                        break;

                    case "FlipHorizontally":                        
                        oi.flipH = ExtractBool(child);
                        break;

                    case "FlipVertically":
                        oi.flipV = ExtractBool(child);
                        break;

                    case "Origin":
                        oi.origin = ExtractVector2(child);
                        break;

                    case "asset_name":
                        try
                        {
                            oi.texturePath = child.FirstChild.Value;
                            oi.texture = cm.Load<Texture2D>(oi.texturePath);
                        }
                        //This causes major performance issues at the moment
                        catch (ContentLoadException e)
                        {
                            bool hej = true;
                        }

                        String[] splitString = child.FirstChild.Value.Split('\\');
                        oi.textureName = splitString[splitString.Length - 1];
 
                        String name = GetObjectName(textureItem);
                        oi.name = name;
                        break;
                }
            }

            Vector2 middle = Vector2.Zero;
            if (oi.texture != null)
            {
                middle = new Vector2(oi.texture.Width / 2, oi.texture.Height / 2);
                Vector2 offset = middle - oi.origin;
                oi.position += offset;
            }
            //oi.origin *= oi.scale;
            objectProcessor.ProcessTextureObject(oi);
        }

        private static void ParseRectangleItem(XmlNode rectangleItem, Vector2 scrollSpeed, String layerName)
        {
            ObjectInformation oi = new ObjectInformation();
            oi.scrollSpeed = scrollSpeed;
            oi.layer = layerName;
            oi.name = GetObjectName(rectangleItem);

            foreach (XmlNode child in rectangleItem.ChildNodes)
            {
                switch (child.Name)
                {
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
                }
            }

            //Adjust the position since Gleed handles textures' and primitives' positions differently.
            oi.position += new Vector2(oi.width/2, oi.height/2);

            objectProcessor.ProcessPrimitiveObject(oi);
        }

        private static String GetLayerName(XmlNode layer)
        {
            String[] splitOuterXml = layer.OuterXml.Split('"');
            String layerName = (String)splitOuterXml.GetValue(1);
            return layerName;
        }

        private static String GetObjectName(XmlNode obj)
        {
            String[] splitOuterXml = obj.OuterXml.Split('"');
            String objectName = (String)splitOuterXml.GetValue(3);
            return objectName;
        }

        private static Vector2 ExtractVector2(XmlNode target)
        {
            String sX = target.FirstChild.FirstChild.InnerText;
            String sY = target.LastChild.FirstChild.InnerText;

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
                foreach (XmlNode customProperty in target.ChildNodes)
                {
                    customProperties.Add(customProperty.FirstChild.InnerText);
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