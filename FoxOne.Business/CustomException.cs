using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
namespace FoxOne.Business
{
    public class PageNotFoundException : FoxOneException
    {
        public PageNotFoundException()
            : base("Page_Not_Found")
        { }
    }

    public class UnAuthorizedException : FoxOneException
    {
        public UnAuthorizedException()
            : base("No_Authentication")
        { }
    }
}
