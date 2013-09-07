using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orujin.Core.Input;

namespace Orujin.Framework
{
    public class OrujinMenu
    {
        public string name { get; private set; }
        protected List<GameObject> menuItems = new List<GameObject>();

        public OrujinMenu(string name)
        {
            this.name = name;
        }


        public virtual OrujinMenu CreateInstance()
        {
            return new OrujinMenu("Base");
        }

        public virtual void Update(float elapsedTime)
        {
            foreach (GameObject go in this.menuItems)
            {
                go.Update(elapsedTime);
            }
        }

        public void AddItem(GameObject item)
        {
            this.menuItems.Add(item);
        }

        public virtual void FadeIn(float time)
        {
        }

        public void FadeOut(float time)
        {
            foreach (GameObject go in this.menuItems)
            {
                go.rendererComponents.FadeOut(time);
            }
        }


        public virtual void Reset()
        {
        }

        public List<GameObject> GetGameObjects()
        {
            return menuItems;
        }
    }
}
