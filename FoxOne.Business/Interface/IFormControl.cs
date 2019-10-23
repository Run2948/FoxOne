using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public interface IFormControl : IComponent
    {
        string Name { get; set; }

        string Validator { get; set; }

        bool Enable { get; set; }

        string Value { get; set; }
    }
}
