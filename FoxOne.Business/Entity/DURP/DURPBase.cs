using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Data.Attributes;
using System.ComponentModel;
namespace FoxOne.Business
{
    [Serializable]
    public abstract class EntityBase : IEntity, IComparable<EntityBase>
    {
        [Column(Showable = false, DataType = "varchar", Length = "38")]
        public virtual string Id { get; set; }

        [Column(Showable = false)]
        public virtual int RentId { get; set; }

        public override bool Equals(object obj)
        {
            return (obj as EntityBase).Id.Equals(this.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(EntityBase other)
        {
            if (this is ISortable)
            {
                return (this as ISortable).Rank.CompareTo((other as ISortable).Rank);
            }
            return this.Id.CompareTo(other.Id);
        }
    }
    public abstract class DURPBase : EntityBase, IDURP, IExtProperty
    {
        [Column(Searchable = true)]
        [DisplayName("名称")]
        public virtual string Name
        {
            get;
            set;
        }

        [DisplayName("别名")]
        public virtual string Alias
        {
            get;
            set;
        }

        [DisplayName("编码")]
        public virtual string Code
        {
            get;
            set;
        }

        [Column(Showable = false)]
        [DisplayName("排序号")]
        public virtual int Rank
        {
            get;
            set;
        }

        [DisplayName("状态")]
        [Column(DataType = "varchar", Length = "10", Searchable = true)]
        [EnumDataSource(typeof(YesOrNo))]
        public virtual string Status
        {
            get;
            set;
        }

        [Column(Showable = false)]
        [DisplayName("最后更新时间")]
        public DateTime LastUpdateTime
        {
            get;
            set;
        }

        private IDictionary<string, object> _Properties;
        public virtual IDictionary<string, object> Properties
        {
            get
            {
                if (_Properties == null)
                {
                    var result = DBContext<IDURPProperty>.Instance.Where(o => o.Type.Equals(Id, StringComparison.OrdinalIgnoreCase));
                    _Properties = result.ToDictionary(o => o.Name, j => j.Value.ConvertTo<object>());
                    if (_Properties == null)
                    {
                        _Properties = new FoxOneDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    }
                }
                return _Properties;
            }
        }

        public void SetProperty()
        {
            foreach (var key in Properties.Keys)
            {
                var item = ObjectHelper.GetObject<IDURPProperty>();
                item.Id = this.GetType().FullName + key;
                item.Name = key;
                item.Value = Properties[key].ToString();
                item.RentId = this.RentId;
                item.Type = this.Id;
                if (!DBContext<IDURPProperty>.Update(item))
                {
                    DBContext<IDURPProperty>.Insert(item);
                }
            }
        }
    }


    public abstract class RelateEntityBase : EntityBase
    {
        [Column(Length = "10")]
        public string Status { get; set; }
    }
}
