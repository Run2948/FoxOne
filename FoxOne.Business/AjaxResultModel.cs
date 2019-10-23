using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public class AjaxResultModel
    {
        public object Data { get; set; }

        public bool Result { get; set; }

        public string ErrorMessage { get; set; }

        public bool NoAuthority { get; set; }

        public bool LoginTimeOut { get; set; }

        public AjaxResultModel()
        {
            this.Result = true;
            this.ErrorMessage = string.Empty;
            this.NoAuthority = false;
            this.LoginTimeOut = false;
        }
    }
}
