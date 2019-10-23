using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using FoxOne.Data.Mapping;

namespace FoxOne.Data
{
    /// <summary>
    /// 数据库访问框架的入口抽象类，定义了用于数据库访问的常用方法
    /// </summary>
    public abstract class Dao
    {
        #region 静态入口方法
        /// <summary>
        /// 获取绑定了默认数据库连接串的<c>Dao</c>对象. 
        /// <para/>
        /// <para>此方法等同于<see cref="M:FoxOne.Data.DaoFactory.GetDao">DaoFactory.GetDao()</see>.</para>
        /// </summary>
        /// <remarks>
        /// 默认数据库连接串的名称约定为"<c>DefaultDB</c>"，必须在配置文件中进行配置
        /// </remarks>
        /// <returns>
        /// 绑定了默认数据库连接的Dao对象
        /// </returns>
        public static Dao Get()
        {
            return DaoFactory.GetDao();
        }

        /// <summary>
        /// 获取绑定了指定名称数据库连接串的<c>Dao</c>对象。 
        /// <para> </para>
        /// <para>此方法等同于<see cref="M:FoxOne.Data.DaoFactory.GetDao(System.String)">DaoFactory.GetDao(string name)</see></para>
        /// </summary>
        /// <param name="name">配置文件中连接串的名称</param>
        /// <returns>
        /// Dao对象
        /// </returns>
        public static Dao Get(string name)
        {
            return DaoFactory.GetDao(name);
        }

        /// <summary>
        /// 获取指定连接串和数据库Provider的数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="providerName">数据库Provider</param>
        /// <returns>数据库连接</returns>
        public static Dao Get(string connectionString, string providerName)
        {
            return DaoFactory.GetDao(connectionString, providerName);
        }
        #endregion

        #region 接口属性
        /// <summary>
        /// 获取绑定相同连接串的企业库<see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.Database">Database</see>对象
        /// </summary>
        public abstract Database Database { get; }

        /// <summary>
        /// 获取当前Dao实例的IDaoProvider对象
        /// </summary>
        public abstract IDaoProvider Provider { get; }

        /// <summary>
        /// 获取当前Dao实例的IMappingProvider对象
        /// </summary>
        public abstract IMappingProvider MappingProvider { get; }

        /// <summary>
        /// 获取当期Dao实例绑定的连接串
        /// </summary>
        public abstract string ConnectionString { get; }

        /// <summary>
        /// 获取当前Dao实例绑定的连接串对应的db providerName
        /// </summary>
        public abstract string DbProviderName { get; }
        #endregion

        #region 数据库更新接口方法
        /// <summary>
        /// 执行非查询类的DML语句和所有的DDL语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>影响到的记录条数</returns>
        public abstract int ExecuteNonQuery(string sql, object parameters = null);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract int BatchInsert(DataTable table);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract int BatchInsert(IList<IDictionary<string, object>> data, string tableName);

        public abstract int BatchUpdate(object entity, object parameter);

        public abstract int BatchUpdate<T>(object entity, object parameter);

        public abstract int BatchDelete(Type type, object parameter);

        public abstract int BatchDelete<T>(object parameter);
        #endregion

        #region 存储过程接口方法
        public abstract int ExecuteProcedure(string procedureName, params object[] parameters);
        #endregion

        #region 查询多行数据接口方法
        /// <summary>
        /// 查询多条数据，并以IDataReader返回
        /// </summary>
        /// <param name="sql">select sql语句</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以IDataReader返回结果集</returns>
        public abstract IDataReader QueryReader(string sql, object parameters = null);

        /// <summary>
        /// 查询多条数据，并以DataSet返回
        /// </summary>
        /// <param name="sql">select sql语句</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以DataSet返回结果集</returns>
        public abstract DataSet QueryDataSet(string sql, object parameters = null);

        /// <summary>
        /// 查询多条数据，并以IDictionary为元素的IList返回
        /// </summary>
        /// <param name="sql">select sql语句</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以IDictionary为元素的IList返回结果集，数据不存在则返回空IList</returns>
        public abstract IList<IDictionary<string, object>> QueryDictionaries(string sql, object parameters = null);

        /// <summary>
        /// 查询多条数据，并以制定元素类型的IList返回
        /// </summary>
        /// <typeparam name="T">结果集的元素类型</typeparam>
        /// <param name="sql">select sql语句</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以实体类为元素的IList返回结果集，数据不存在则返回空IList</returns>
        public abstract IList<T> QueryEntities<T>(string sql, object parameters = null)
            where T : class, new();

        /// <summary>
        /// 查询多条数据，并以制定元素类型的IList返回
        /// </summary>
        /// <typeparam name="T">结果集的元素类型（可以是接口）</typeparam>
        /// <param name="type">结果集的元素类型（一般是实现类）</param>
        /// <param name="sql">select sql语句</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以实体类为元素的IList返回结果集，数据不存在则返回空IList</returns>
        public abstract IList<T> QueryEntities<T>(Type type, string sql, object parameters = null)
            where T : class;

        #endregion

        #region 查询一行数据接口方法
        /// <summary>
        /// 查询单条数据，并以IDictionary返回
        /// </summary>
        /// <param name="sql">查询单条数据的select sql语句</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以IDictionary返回单条数据，数据不存在则返回Null</returns>
        public abstract IDictionary<string, object> QueryDictionary(string sql, object parameters = null);

        /// <summary>
        /// 查询单条数据，并以实体类返回
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="sql">查询单条数据的select sql语句（字段名必须与实体类属性名一致）</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以实体类返回单条数据，数据不存在则返回Null</returns>
        public abstract T QueryEntity<T>(string sql, object parameters = null) where T : class, new();

        /// <summary>
        /// 查询单条数据，并以实体类返回
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="type">实体类接口</param>
        /// <param name="sql">查询单条数据的select sql语句（字段名必须与实体类属性名一致）</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以实体类返回单条数据，数据不存在则返回Null</returns>
        public abstract T QueryEntity<T>(Type type, string sql, object parameters = null);
        #endregion

        #region 查询单行单列接口方法
        /// <summary>
        /// 执行select sql语句，检查是否有数据返回
        /// </summary>
        /// <param name="sql">select sql语句</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>True有数据 false没有数据</returns>
        public abstract bool Exists(string sql, object parameters = null);

        /// <summary>
        /// 查询单条数据的单个字段
        /// </summary>
        /// <typeparam name="T">单个字段的返回类型</typeparam>
        /// <param name="sql">查询单个字段的sql语句</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以指定的类型返回制定字段。在查询出来字段值为空的情况时，如果指定以string或其他类型返回，则返回null，如果以int等数值型类型返回，则返回0</returns>
        public abstract T QueryScalar<T>(string sql, object parameters = null);

        /// <summary>
        /// 查询多条数据的单个字段
        /// </summary>
        /// <typeparam name="T">单个字段的返回类型</typeparam>
        /// <param name="sql">查询多条数据单个字段的sql语句</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以制定元素的IList返回多条数据的单个字段，数据不存在在返回空IList</returns>
        public abstract IList<T> QueryScalarList<T>(string sql, object parameters = null);
        #endregion

        #region ORMapping接口方法

        /// <summary>
        /// 根据实体类型创建数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract void CreateTable<T>();

        /// <summary>
        /// 根据实体类型创建数据表
        /// </summary>
        /// <param name="type"></param>
        public abstract void CreateTable(Type type);

        /// <summary>
        /// 查询单条数据，以对象方式返回
        /// </summary>
        /// <typeparam name="T">单条数据所对应的对象类型</typeparam>
        /// <param name="id">对于以一个字段作为关键字的表，传入单个字段的值；对于多个字段作为关键字的表，则需要以Dictionary或者Entity的方式传入</param>
        public abstract T Get<T>(object id) where T : class;

        /// <summary>
        /// 查询单条数据，以对象方式返回
        /// </summary>
        /// <param name="type">单条数据所对应的对象类型</param>
        /// <param name="id">对于以一个字段作为关键字的表，传入单个字段的值；对于多个字段作为关键字的表，则需要以Dictionary或者Entity的方式传入</param>
        /// <returns></returns>
        public abstract object Get(Type type, object id);

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="tableName">数据表名</param>
        /// <param name="id">对于以一个字段作为关键字的表，传入单个字段的值；对于多个字段作为关键字的表，则需要以Dictionary或者Entity的方式传入</param>
        /// <returns></returns>
        public abstract IDictionary<string,object> Get(string tableName, object id);

        /// <summary>
        /// 查询表所有的数据，以集合方式返回
        /// </summary>
        /// <typeparam name="T">单条数据所对应的对象类型</typeparam>
        public abstract IList<T> Select<T>(object parameter = null) where T : class;

        /// <summary>
        /// 查询表所有的数据，以集合方式返回
        /// </summary>
        /// <typeparam name="T">单条数据所对应的对象类型</typeparam>
        /// <param name="parameter">参数</param>
        public abstract IList<object> Select(Type type, object parameter = null);

        /// <summary>
        /// 查询表所有的数据，以集合方式返回
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public abstract IList<IDictionary<string, object>> Select(string tableName, object parameter = null);

        /// <summary>
        /// 检查指定关键字值的数据是否存在
        /// </summary>
        /// <typeparam name="T">单条数据所对应的对象类型</typeparam>
        /// <param name="id">对于以一个字段作为关键字的表，传入单个字段的值；对于多个字段作为关键字的表，则需要以Dictionary或者Entity的方式传入</param>
        public abstract bool Exists<T>(object id);

        /// <summary>
        /// 检查制定对象在数据库中是否有对应的数据
        /// </summary>
        /// <param name="entity">代表单条数据的对象，必须是实体类的实例</param>
        public abstract bool Exists(object entity);

        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <param name="entity">代表单条数据的实体类实例</param>
        /// <returns>成功影响数据的条数，插入成功则返回1，否则返回0</returns>
        public abstract int Insert(object entity);

        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <param name="tableName">数据表名</param>
        /// <param name="entity">数据</param>
        /// <returns>成功影响数据的条数，插入成功则返回1，否则返回0</returns>
        public abstract int Insert(string tableName, object entity);

        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <typeparam name="T">单条数据实体对应的对象类型</typeparam>
        /// <param name="entity">代表单条数据的对象，必须是实体类的实例</param>
        /// <returns>成功影响数据的条数，真正成功删除到指定数据返回1，否则返回0</returns>
        public abstract int Insert<T>(object entity);

        /// <summary>
        /// 插入单调数据
        /// </summary>
        /// <typeparam name="T">与数据表相对应的实体对象类型</typeparam>
        /// <param name="fields">要插入的单条数据，key必须与实体对象属性或者数据库表字段名一致</param>
        /// <returns>成功影响数据的条数，真正成功删除到指定数据返回1，否则返回0</returns>
        public abstract int InsertFields<T>(IDictionary<String, Object> fields);

        /// <summary>
        /// 更新单条数据
        /// </summary>
        /// <param name="entity">代表单条数据的实体类实例</param>
        /// <returns>成功影响数据的条数，真正成功更新到指定数据返回1，否则返回0</returns>
        public abstract int Update(object entity);

        /// <summary>
        /// 更新单条数据
        /// </summary>
        /// <param name="tableName">数据表名</param>
        /// <param name="entity">要更新的数据</param>
        /// <returns></returns>
        public abstract int Update(string tableName, object entity);

        /// <summary>
        /// 更新单条数据
        /// </summary>
        /// <typeparam name="T">与数据库表对应的对象实体类型</typeparam>
        /// <param name="entity">代表单条数据的实体类实例</param>
        /// <returns>成功影响数据的条数，真正成功更新到指定数据返回1，否则返回0</returns>
        public abstract int Update<T>(Object entity);

        /// <summary>
        /// 更新单条数据
        /// </summary>
        /// <typeparam name="T">与数据库表对应的对象实体类型</typeparam>
        /// <param name="fields">代表单条数据的实体类型实例</param>
        /// <returns>成功影响数据的条数，真正成功更新到指定数据返回1，否则返回0</returns>
        public abstract int UpdateFields<T>(IDictionary<String, Object> fields);

        /// <summary>
        /// 更新单条数据，指定需要更新的属性
        /// </summary>
        /// <typeparam name="T">与数据库表对应的对象实体类型</typeparam>
        /// <param name="entity">代表单条数据的实体类型实例</param>
        /// <param name="inclusiveFields">需要更新的字段名称或者数据库字段名称</param>
        /// <returns>成功影响数据的条数，真正成功更新到指定数据返回1，否则返回0</returns>
        public abstract int UpdateFields<T>(Object entity, params String[] inclusiveFields);

        /// <summary>
        /// 更新单条数据，指定需要更新的属性
        /// </summary>
        /// <param name="entity">代表单条数据的实体类型实例</param>
        /// <param name="inclusiveFields">需要更新的字段名称或者数据库字段名称</param>
        /// <returns>成功影响数据的条数，真正成功更新到指定数据返回1，否则返回0</returns>
        public abstract int UpdateFields(Object entity, params String[] inclusiveFields);

        /// <summary>
        /// 更新单条数据，指定不需要更新的属性
        /// </summary>
        /// <param name="entity">代表单条数据的实体类型实例</param>
        /// <param name="exclusiveFields">不需要更新的字段名称或者数据库字段名称</param>
        /// <returns>成功影响数据的条数，真正成功更新到指定数据返回1，否则返回0</returns>
        public abstract int UpdateFieldsExcluded(Object entity, params String[] exclusiveFields);

        /// <summary>
        /// 更新单条数据，指定不需要更新的属性
        /// </summary>
        /// <typeparam name="T">与数据库表对应的实体类型</typeparam>
        /// <param name="entity">代表单条数据的实体类型实例</param>
        /// <param name="exclusiveFields">不需要更新的字段名称或者数据库字段名称</param>
        /// <returns>成功影响数据的条数，真正成功更新到指定数据返回1，否则返回0</returns>
        public abstract int UpdateFieldsExcluded<T>(Object entity, params String[] exclusiveFields);

        /// <summary>
        /// 更新单条数据，只更新值不为null的属性到数据库对应字段里
        /// </summary>
        /// <param name="entity">代表单条数据的实体类型实例</param>
        /// <returns>成功影响数据的条数，真正成功更新到指定数据返回1，否则返回0</returns>
        public abstract int UpdateFieldsNotNull(Object entity);

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="id">对于以一个字段作为关键字的表，传入单个字段的值；对于多个字段作为关键字的表，则需要以Dictionary或者Entity的方式传入</param>
        /// <returns>成功影响数据的条数，真正成功删除到指定数据返回1，否则返回0</returns>
        public abstract int Delete<T>(object id);

        /// <summary>
        /// 删除掉数据库中与对象实例对应的数据
        /// </summary>
        /// <param name="entity">代表单条数据的对象，必须是实体类的实例</param>
        /// <returns>成功影响数据的条数，真正成功删除到指定数据返回1，否则返回0</returns>
        public abstract int Delete(object entity);

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="tableName">数据表名</param>
        /// <param name="id">对于以一个字段作为关键字的表，传入单个字段的值；对于多个字段作为关键字的表，则需要以Dictionary或者Entity的方式传入</param>
        /// <returns></returns>
        public abstract int Delete(string tableName, object id);
        #endregion

        #region 分页查询接口方法

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="sql">分页查询sql语句</param>
        /// <param name="startRowIndex">从第几条数据开始</param>
        /// <param name="maximumRows">获取多少条数据，0表示获取startRowIndex之后的所有数据</param>
        /// <param name="sortExpression">排序表达式，例如：field1 asc</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以DataSet的格式返回分页数据</returns>
        public abstract DataSet
            PageQueryDataSet(string sql, int startRowIndex, int maximumRows, String sortExpression, object parameters = null);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="sql">分页查询sql语句</param>
        /// <param name="startRowIndex">从第几条数据开始</param>
        /// <param name="maximumRows">获取多少条数据，0表示获取startRowIndex之后的所有数据</param>
        /// <param name="sortExpression">排序表达式，例如：field1 asc</param>
        /// <param name="totalRowCount">输出记录总数</param>
        /// <param name="parameters">普通Object或者IDictionary，分别通过属性名和Key与sql中的参数名匹配</param>
        /// <returns>以DataSet的格式返回分页数据</returns>
        public abstract DataSet
            PageQueryDataSet(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, object parameters = null);

        public abstract IList<IDictionary<string, object>>
            PageQueryDictionaries(string sql, int startRowIndex, int maximumRows, String sortExpression, object parameters = null);

        public abstract IList<IDictionary<string, object>>
            PageQueryDictionaries(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, object parameters = null);

        public abstract IList<T>
            PageQueryEntities<T>(string sql, int startRowIndex, int maximumRows, String sortExpression, object parameters = null)
            where T : class,new();

        public abstract IList<T>
            PageQueryEntities<T>(string sql, int startRowIndex, int maximumRows, String sortExpression, out int totalRowCount, object parameters = null)
            where T : class,new();

        public abstract DataSet
            PageQueryDataSetByPage(string sql, int page, int size, string sortExpression, object parameters = null);

        public abstract DataSet
            PageQueryDataSetByPage(string sql, int page, int size, string sortExpression, out int totalRowCount, object parameters = null);

        public abstract IList<IDictionary<string, object>>
            PageQueryDictionariesByPage(string sql, int page, int size, string sortExpression, object parameters = null);

        public abstract IList<IDictionary<string, object>>
            PageQueryDictionariesByPage(string sql, int page, int size, String sortExpression, out int totalRowCount, object parameters = null);

        public abstract IList<T>
            PageQueryEntitiesByPage<T>(string sql, int page, int size, String sortExpression, object parameters = null)
            where T : class,new();

        public abstract IList<T>
            PageQueryEntitiesByPage<T>(string sql, int page, int size, String sortExpression, out int totalRowCount, object parameters = null)
            where T : class,new();
        #endregion

        #region 内部接口方法
        internal abstract int ExecuteNonQuery(ISqlCommand command, String connectionName);

        internal abstract DataSet QueryDataSet(ISqlCommand command, String connectionName);

        internal abstract IDataReader QueryReader(ISqlCommand command, String connectionName);

        internal abstract T QueryScalar<T>(ISqlCommand command, String connectionName);
        #endregion
    }
}