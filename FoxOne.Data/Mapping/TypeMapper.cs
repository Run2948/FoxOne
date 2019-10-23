using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using FoxOne.Core;
namespace FoxOne.Data.Mapping
{
    public class TypeMapper
    {

        #region 接口方法
        public static T Read<T>(IDictionary<string,object> data) where T : class,new()
        {
            return Read<T>(data, typeof (T));
        }

        public static T Read<T>(IDictionary<string,object> data, Type instanceType)
        {
            if (null != data)
            {
                FastType reflection = FastType.Get(instanceType);

                object instance = Activator.CreateInstance(instanceType);

                foreach (String key in data.Keys)
                {
                    FastProperty prop =
                        reflection.Setters.SingleOrDefault(p => MatchColumnName(p.Name, key));

                    if (null != prop)
                    {
                        prop.SetValue(instance,data[key].ConvertToType(prop.Type));
                    }
                }
                return (T)instance;
            }
            else
            {
                return default(T);
            }
        }

        public static T Read<T>(IDataReader reader) where T : class,new()
        {
            return Read<T>(reader, typeof(T));
        }

        public static T Read<T>(IDataReader reader, Type instanceType)
        {
            if (reader.Read())
            {
                return Read<T>(reader, instanceType, GetSetterMappings(instanceType, reader));
            }
            else
            {
                return default(T);
            }
        }

        public static IList<T> ReadList<T>(IDataReader reader) where T : class,new()
        {
            return ReadList<T>(reader, typeof(T));
        }

        public static IList<T> ReadList<T>(IDataReader reader, Type instanceType)
            where T : class
        {
            IList<T> list = new List<T>();

            if (reader.Read())
            {
                PropertyMapping[] mappings = GetSetterMappings(instanceType, reader);

                do
                {
                    list.Add(Read<T>(reader, instanceType, mappings));
                }
                while (reader.Read());
            }
            return list;
        }
        #endregion

        #region 内部实现
        private static T Read<T>(IDataReader reader,
                                 Type type,
                                 PropertyMapping[] mappings)
        {
            object instance = Activator.CreateInstance(type);

            foreach (PropertyMapping mapping in mappings)
            {
                FastProperty prop = mapping.Prop;

                prop.SetValue(instance, reader.GetValue(mapping.Index).ConvertToType(prop.Type));
            }
            return (T)instance; 
        }

        //private static readonly Dictionary<string,PropertyMapping[]> _mappingCache = new Dictionary<string,PropertyMapping[]>();
        //private static readonly ReaderWriterLockSlim _mappingLock = new ReaderWriterLockSlim();

        private static PropertyMapping[] GetSetterMappings(Type type, IDataReader reader, string mappingKey = null)
        {
            PropertyMapping[] mappings = null;

            //read from cache
            /*TODO : 暂不考虑缓存映射结果
            if (null != mappingKey)
            {
                _mappingLock.EnterReadLock();
                try
                {
                    if (_mappingCache.TryGetValue(mappingKey, out mappings))
                    {
                        return mappings;
                    }
                }
                finally
                {
                    _mappingLock.ExitReadLock();
                }
            }
            */

            FastType reflection = FastType.Get(type);
            List <PropertyMapping> list = new List<PropertyMapping>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);

                FastProperty prop = 
                    reflection.Setters.SingleOrDefault(m => MatchColumnName(m.Name,columnName));

                if (prop != null)
                {
                    list.Add(new PropertyMapping() { Prop = prop, Index = i });
                }
            }

            mappings = list.ToArray();

            /*
            if (null != mappingKey)
            {
                _mappingLock.EnterWriteLock();
                try
                {
                    _mappingCache[mappingKey] = mappings;
                }
                finally
                {
                    _mappingLock.ExitWriteLock();
                }
            }
            */

            return mappings;
        }

        private static bool MatchColumnName(string name, string columnName)
        {
            return columnName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                   columnName.Replace(" ", "").Replace("_", "").Equals(name, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        internal struct PropertyMapping
        {
            public FastProperty Prop;
            public int Index;
        }
    }
}