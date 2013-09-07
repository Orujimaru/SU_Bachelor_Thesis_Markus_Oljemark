using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orujin.Framework
{
    public class OrujinHud
    {
        protected List<GameObject> hudItems = new List<GameObject>();

        public OrujinHud()
        {
        }

        public virtual void Update(float elapsedTime)
        {
            foreach (GameObject go in this.hudItems)
            {
                go.Update(elapsedTime);
            }
        }

        public void AddItem(GameObject item)
        {
            this.hudItems.Add(item);
        }

        public virtual void ActivateItem(string name)
        {
        }

        public List<GameObject> GetGameObjects()
        {
            return hudItems;
        }
    }
}
