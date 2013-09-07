using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orujin.Framework
{
    public abstract class OrujinLevel
    {
        public string name { get; private set; }

        public OrujinLevel(string name)
        {
            this.name = name;
        }

        public abstract void Initialize();

        public abstract void Update(float elapsedTime);
    }
}
