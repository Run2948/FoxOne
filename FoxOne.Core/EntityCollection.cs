using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FoxOne.Core
{
    public class EntityCollection<TEntity>:List<TEntity> where TEntity : class,IEntity
    {
        public EntityCollection()
            : base(ObjectHelper.GetObject<IService<TEntity>>().Select())
        {
        }

        public TEntity Get(string id)
        {
            return this.FirstOrDefault(o => o.Id.Equals(id.ToString(), StringComparison.OrdinalIgnoreCase));
        }
    }


}
