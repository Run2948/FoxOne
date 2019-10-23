using FoxOne.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace FoxOne.Business
{
    [DisplayName("实体转换器")]
    public class EntityColumnConverter:ColumnConverterBase
    {

        private string entityTypeFullName;
        private Type _entityType;
        private EntityDataSource _dataSource;

        [DisplayName("实体")]
        [TypeDataSource(typeof(IAutoCreateTable))]
        [FormField(ControlType = ControlType.DropDownList)]
        public string EntityTypeFullName
        {
            get
            {
                return entityTypeFullName;
            }
            set
            {
                entityTypeFullName = value;
                EntityType = TypeHelper.GetType(value);

            }
        }

        [ScriptIgnore]
        public Type EntityType
        {
            get
            {
                return _entityType;
            }
            set
            {
                _entityType = value;
            }
        }
        protected override IFieldConverter FieldConverter
        {
            get
            {
                return _dataSource ?? (_dataSource = new EntityDataSource() { EntityTypeFullName = EntityTypeFullName });
            }
        }
    }
}
