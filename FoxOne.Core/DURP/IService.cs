using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface IService<T>
    {
        T Get(object id);

        IEnumerable<T> Select(object parameter=null);

        int Insert(T entity);

        int Update(T entity);

        int Delete(object id);
    }
}
