using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public class ValidatorAttribute:Attribute
    {
        public string ValidateString { get; set; }

        public ValidatorAttribute(string validateString)
        {
            ValidateString = validateString;
        }

    }
}
