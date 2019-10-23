using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FoxOne.Core
{
    public static class DBContext<TEntity> where TEntity : class, IEntity
    {
        public static EntityCollection<TEntity> Instance
        {
            get
            {
                var type = ObjectHelper.GetRegisterType<TEntity>();
                return CacheHelper.GetFromCache<EntityCollection<TEntity>>(type.FullName, () => {
                    return new EntityCollection<TEntity>();
                });
            }
        }

        public static bool ClearCache()
        {
            var type = ObjectHelper.GetRegisterType<TEntity>();
            return CacheHelper.Remove(type.FullName);
        }

        private static IService<TEntity> _service = null;
        private static IService<TEntity> Service
        {
            get
            {
                return _service ?? (_service = ObjectHelper.GetObject<IService<TEntity>>());
            }
        }

        public static bool Insert(TEntity item)
        {
            var result =  Service.Insert(item) > 0;
            if(result)
            {
                ClearCache();
            }
            return result;
        }

        public static bool Update(TEntity item)
        {
            var result = Service.Update(item) > 0;
            if (result)
            {
                ClearCache();
            }
            return result;
        }

        public static bool Delete(object item)
        {
            var result = Service.Delete(item) > 0;
            if (result)
            {
                ClearCache();
            }
            return result;
        }

        public static TEntity Get(string id)
        {
            return Service.Get(id);
        }
    }

    public static class EntityEventManager
    {
        private static IDictionary<string, Func<object, bool>> EntityEventList = new Dictionary<string, Func<object, bool>>();
        private const string KeyTemplate = "{0}_{1}_{2}";
        public static bool RaiseEvent<TEntity>(EventStep step, EventType type, TEntity o)
        {
            string key = KeyTemplate.FormatTo(step.ToString(), type.ToString(), o.GetType().FullName);
            Logger.Debug("Raise Event:{0}".FormatTo(key));
            if (EntityEventList.ContainsKey(key))
            {
                return EntityEventList[key](o);
            }
            return true;
        }

        public static void RegisterEvent<TEntity>(EventStep step, EventType type, Func<object, bool> predicate)
        {
            if (predicate != null)
            {
                var tfromType = ObjectHelper.GetRegisterType<TEntity>();
                string key = KeyTemplate.FormatTo(step.ToString(), type.ToString(), tfromType.FullName);
                Logger.Debug("Register Event:{0}".FormatTo(key));
                EntityEventList.Add(key, predicate);
            }
        }
    }

    public enum EventType
    {
        Insert,
        Update,
        Delete
    }

    public enum EventStep
    {
        Before,
        After
    }
}
