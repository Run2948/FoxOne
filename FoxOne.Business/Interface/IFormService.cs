using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public interface IFormService : IControl
    {
        int Insert(IDictionary<string, object> data);

        int Update(string key, IDictionary<string, object> data);

        IDictionary<string, object> Get(string key);

        int Delete(string key);
    }
}
