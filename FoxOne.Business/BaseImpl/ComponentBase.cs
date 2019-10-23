using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public abstract class ComponentBase : ControlBase, IComponent, ICloneable
    {

        public string CssClass
        {
            get;
            set;
        }

        public bool Visiable
        {
            get;
            set;
        }

        public abstract string Render();

        public int Rank
        {
            get;
            set;
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
