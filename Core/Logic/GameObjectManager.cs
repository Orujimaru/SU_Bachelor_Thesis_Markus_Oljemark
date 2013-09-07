using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Orujin.Core.Physics;
using Orujin.Core.GameHelp;
using Orujin.Core.Input;
using Orujin.Core.Renderer;
using Orujin.Framework;
using FarseerPhysics.Dynamics;
using Orujin.Debug;

namespace Orujin.Core.Logic
{
    public class GameObjectManager
    {

        private List<GameObject> gameObjects = new List<GameObject>();
        private List<GameObject> garbageList = new List<GameObject>();

        public GameObjectManager()
        {

        }

        public void Update(float elapsedTime)
        {
            for(int x = 0; x < gameObjects.Count(); x++)
            {
                this.gameObjects[x].Update(elapsedTime);

                if (this.gameObjects[x].checkForPixelCollision)
                {
                    //Check all objects infront
                    for (int y = x + 1; y < this.gameObjects.Count(); y++)
                    {
                        if (this.gameObjects[y].checkForPixelCollision)
                        {
                           // this.CheckForPixelCollision(this.gameObjects[x], this.gameObjects[y]);
                        }
                    }
                }
                //Remove everything that is in the garbageList
                foreach (GameObject garbage in this.garbageList)
                {
                    garbage.Destroy();
                    this.gameObjects.Remove(garbage);
                }
                this.garbageList.Clear();
            }
        }

        private void CheckForPixelCollision(GameObject objectA, GameObject objectB)
        {
            List<Sprite> objectAComponents = objectA.rendererComponents.GetChildren();
            List<Sprite> objectBComponents = objectB.rendererComponents.GetChildren();

            for (int aX = 0; aX < objectAComponents.Count(); aX++)
            {
                for (int bX = 0; bX < objectBComponents.Count(); bX++)
                {
                    if (PixelCollisionManager.Intersects(objectAComponents[aX], objectBComponents[bX]))
                    {
                        bool hej = true;
                    }
                }
            }
        }

        public void CheckForInput(InputCommand ic)
        {
            foreach (GameObject go in this.gameObjects)
            {
                if (go.checkForInput)
                {
                    if (go.identity.name.Equals(ic.objectName))
                    {
                        MethodInfo method = go.GetType().GetMethod(ic.methodName);
                        if (ic.thumbstick)
                        {
                            method.Invoke(go, DynamicArray.ObjectArray(ic.parameters, ic.magnitude));
                        }
                        else
                        {
                            method.Invoke(go, ic.parameters);
                        }
                    }
                }
            }
        }

        public GameObject GetByName(string name)
        {
            foreach (GameObject go in this.gameObjects)
            {
                if (go.identity.name.Equals(name))
                {
                    return go;
                }
            }
            return null;
        }

        public List<GameObject> GetByTag(string tag)
        {
            List<GameObject> tempObjects = new List<GameObject>();
            foreach (GameObject go in this.gameObjects)
            {
                if (go.identity.tag.Equals(tag))
                {
                    tempObjects.Add(go);
                }
            }
            return tempObjects;           
        }


        public bool Add(GameObject newObject)
        {
            if (this.gameObjects.Contains(newObject))
            {
                return false;
            }
            this.gameObjects.Add(newObject);
            return true;
        }

        private static Predicate<GameObject> ByIdentity(GameObject other)
        {
            return delegate(GameObject go)
            {
                return (go.identity.tag == other.identity.tag && go.identity.name == other.identity.name);
            };
        }

        public bool AddAfter(GameObject afterThis, GameObject addThis)
        {
            if (this.gameObjects.Contains(addThis))
            {
                return false;
            }
            int index = this.gameObjects.FindIndex(ByIdentity(afterThis));
            this.gameObjects.Insert(index, addThis);
            return true;
        }

        public bool Remove(GameObject removeObject)
        {
            if (this.gameObjects.Contains(removeObject))
            {
                this.garbageList.Add(removeObject);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            foreach (GameObject go in this.gameObjects)
            {
                go.Destroy();
            }
            this.gameObjects.Clear();
            this.garbageList.Clear();
        }

        public List<GameObject> GetGameObjects()
        {
            return this.gameObjects;
        }
    }
}
