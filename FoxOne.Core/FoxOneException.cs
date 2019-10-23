using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public class FoxOneException : Exception
    {
        public FoxOneException()
        {

        }

        public FoxOneException(string message)
            : base(ObjectHelper.GetObject<ILangProvider>().GetString(message))
        {
        }

        public FoxOneException(string format, params string[] param)
            : base(ObjectHelper.GetObject<ILangProvider>().GetString(format).FormatTo(param))
        {
        }

        public FoxOneException(string message, Exception innerException)
            : base(ObjectHelper.GetObject<ILangProvider>().GetString(message), innerException)
        {
        }
    }
}
