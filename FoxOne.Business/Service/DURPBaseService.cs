using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Data;
namespace FoxOne.Business
{
    public class CommonService<TEntity> : IService<TEntity> where TEntity : IEntity
    {
        public virtual TEntity Get(object id)
        {
            var type = ObjectHelper.GetRegisterType<TEntity>();
            return (TEntity)Dao.Get().Get(type, id);
        }

        public virtual IEnumerable<TEntity> Select(object parameter = null)
        {
            var type = ObjectHelper.GetRegisterType<TEntity>();
            var result = Dao.Get().Select(type, parameter);
            foreach (var item in result)
            {
                yield return (TEntity)item;
            }
        }

        public virtual int Insert(TEntity item)
        {
            int result = 0;
            if (item is ILastUpdateTime)
            {
                (item as ILastUpdateTime).LastUpdateTime = DateTime.Now;
            }
            if (item is IEntity)
            {
                var i = item as IEntity;
                if (i.Id.IsNullOrEmpty())
                {
                    i.Id = Guid.NewGuid().ToString();
                    i.RentId = 1;
                }
            }
            EntityEventManager.RaiseEvent<TEntity>(EventStep.Before, EventType.Insert, item);
            result = Dao.Get().Insert(item);
            if (result > 0)
            {
                if (item is IExtProperty)
                {
                    (item as IExtProperty).SetProperty();
                }
                EntityEventManager.RaiseEvent<TEntity>(EventStep.After, EventType.Insert, item);
            }
            return result;
        }

        public virtual int Update(TEntity item)
        {
            if (item is ILastUpdateTime)
            {
                (item as ILastUpdateTime).LastUpdateTime = DateTime.Now;
            }
            if (item is IEntity)
            {
                var i = item as IEntity;
                i.RentId = 1;
            }
            EntityEventManager.RaiseEvent<TEntity>(EventStep.Before, EventType.Update, item);
            int result = Dao.Get().Update(item);
            if (result > 0)
            {
                if (item is IExtProperty)
                {
                    (item as IExtProperty).SetProperty();
                }
                EntityEventManager.RaiseEvent<TEntity>(EventStep.After, EventType.Update, item);
            }
            return result;
        }

        public virtual int Delete(object id)
        {
            TEntity item = default(TEntity);
            if (id is TEntity)
            {
                item = (TEntity)id;
            }
            else
            {
                item = Get(id);
            }
            EntityEventManager.RaiseEvent<TEntity>(EventStep.Before, EventType.Delete, item);
            int result = Dao.Get().Delete(item);
            if (result > 0)
            {
                EntityEventManager.RaiseEvent<TEntity>(EventStep.After, EventType.Delete, item);
            }
            return result;
        }
    }


}
