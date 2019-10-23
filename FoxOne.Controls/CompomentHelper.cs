using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web;
using FoxOne.Core;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Globalization;
using System.IO;
using System.Collections;
using System.ComponentModel;
using FoxOne.Data.Attributes;
namespace FoxOne.Controls
{
    public static class ComponentHelper
    {
        public static void RecSave(IControl instance)
        {
            SaveComponent(instance);
            var pis = FastType.Get(instance.GetType()).Setters;
            foreach (var p in pis)
            {
                if (typeof(IControl).IsAssignableFrom(p.Type))
                {
                    var newInstance = p.GetValue(instance) as IControl;
                    if (newInstance != null)
                    {
                        newInstance.PageId = instance.PageId;
                        newInstance.TargetId = p.Name;
                        newInstance.ParentId = instance.Id;
                        if (newInstance.Id.IsNullOrEmpty())
                        {
                            newInstance.Id = newInstance.ParentId + newInstance.GetType().Name;
                        }
                        RecSave(newInstance);
                    }
                }
                if (p.Type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(p.Type) && !p.Name.Equals("Controls", StringComparison.CurrentCultureIgnoreCase))
                {
                    var lists = p.GetValue(instance) as IEnumerable;
                    if (lists != null)
                    {
                        int i = 0;
                        foreach (var list in lists)
                        {
                            var newInstance = list as IControl;
                            newInstance.PageId = instance.PageId;
                            newInstance.TargetId = p.Name;
                            newInstance.ParentId = instance.Id;
                            if (newInstance.Id.IsNullOrEmpty())
                            {
                                newInstance.Id = newInstance.ParentId + newInstance.GetType().Name + i;
                            }
                            RecSave(newInstance);
                            i++;
                        }
                    }
                }
            }
        }

        public static void SaveComponent(IControl entity, bool isInsert = true)
        {
            var componentEntity = DBContext<ComponentEntity>.Instance.FirstOrDefault(o => o.Id.Equals(entity.Id, StringComparison.OrdinalIgnoreCase)
                && o.PageId.Equals(entity.PageId, StringComparison.OrdinalIgnoreCase));
            if (componentEntity != null)
            {
                if (isInsert)
                {
                    throw new FoxOneException("Ctrl_Id_Repeat", entity.Id);
                }
            }
            else
            {
                componentEntity = new ComponentEntity();
                componentEntity.Id = entity.Id;
                componentEntity.PageId = entity.PageId;
                componentEntity.ParentId = entity.ParentId;
                componentEntity.TargetId = entity.TargetId;
                if (entity is PageControlBase)
                {
                    componentEntity.DataType = "Component";
                }
                else
                {
                    componentEntity.DataType = "Control";
                }
                if ((entity is IFieldConverter) || (entity is IColumnConverter))
                {
                    componentEntity.DataType = "DataSource";
                }
            }
            componentEntity.LastUpdateTime = DateTime.Now;
            if (entity is ISortableControl)
            {
                componentEntity.Rank = (entity as ISortableControl).Rank;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new FoxOne.Business.ComponentConverter() });
            componentEntity.JsonContent = serializer.Serialize(entity);
            if (!isInsert)
            {
                var item = DBContext<ComponentEntity>.Instance.FirstOrDefault(o => o.PageId.Equals(entity.PageId, StringComparison.OrdinalIgnoreCase) && o.Id.Equals(entity.Id, StringComparison.OrdinalIgnoreCase));
                if (item != null && item.Type.Equals(entity.GetType().FullName, StringComparison.OrdinalIgnoreCase))
                {
                    DBContext<ComponentEntity>.Update(componentEntity);
                }
                else
                {
                    componentEntity.Type = entity.GetType().FullName;
                    //如果在设计器中改变了control的类型（原来是DropDownList，变成了TextBox），则需要把原来该控件及其子控件全部删除，然后再添加进去。
                    DeleteComponent(entity.PageId, entity.Id);
                    DBContext<ComponentEntity>.Insert(componentEntity);
                }
            }
            else
            {
                componentEntity.Type = entity.GetType().FullName;
                DBContext<ComponentEntity>.Insert(componentEntity);
            }
        }

        public static void DeleteComponent(string pageId, string ctrlId)
        {
            var ctrlIds = ctrlId.Split(',');
            var allControl = DBContext<ComponentEntity>.Instance.Where(o => o.PageId.Equals(pageId, StringComparison.OrdinalIgnoreCase)).ToList();
            var items = allControl.Where(o => ctrlIds.Contains(o.Id, StringComparer.OrdinalIgnoreCase));
            var children = new List<ComponentEntity>();
            foreach (var item in items)
            {
                RecGet(item, children, allControl);
                children.Add(item);
            }
            foreach (var i in children)
            {
                DBContext<ComponentEntity>.Delete(i);
            }
        }

        private static void RecGet(ComponentEntity item, List<ComponentEntity> result, List<ComponentEntity> allControl)
        {
            var children = allControl.Where(o => o.ParentId.Equals(item.Id, StringComparison.OrdinalIgnoreCase));
            if (!children.IsNullOrEmpty())
            {
                foreach (var child in children)
                {
                    result.Add(child);
                    RecGet(child, result, allControl);
                }
            }
        }

        public static Tab GetTabComponent(object instance, string parentId, string pageId)
        {
            var entityType = instance.GetType();
            var pis = FastType.Get(entityType).Setters;
            var tab = new Tab();
            tab.TabItems.Add(new TabItem()
            {
                Id = "baseInfo",
                TabName = entityType.GetDisplayName(),
                Visiable = true
            });
            var id = (instance as IControl).Id;
            var param = new Dictionary<string, object>();
            param[NamingCenter.PARAM_PAGE_ID] = pageId;
            param[NamingCenter.PARAM_PARENT_ID] = parentId;
            foreach (var fp in pis)
            {
                var p = fp.Info;
                param[NamingCenter.PARAM_TARGET_ID] = p.Name;
                if (p.GetCustomAttribute<FormFieldAttribute>(true) != null || p.GetCustomAttribute<BrowsableAttribute>(true) != null)
                {
                    continue;
                }
                if (p.PropertyType.IsGenericType)
                {
                    if (typeof(IDictionary<string, object>).IsAssignableFrom(p.PropertyType))
                    {
                        continue;
                    }
                    var typeArgument = p.PropertyType.GetGenericArguments()[0];
                    var newTab = new TabItem()
                    {
                        Id = p.Name,
                        TabName = p.GetDisplayName(),
                        Visiable = true
                    };
                    param[NamingCenter.PARAM_BASE_TYPE] = typeArgument.FullName;
                    param[NamingCenter.PARAM_TYPE_NAME] = typeArgument.FullName;
                    newTab.Content.Add(new IFrame() { Src = HttpHelper.BuildUrl(NamingCenter.CTRL_LIST_URL, param) });
                    tab.TabItems.Add(newTab);
                }
                else if (typeof(IControl).IsAssignableFrom(p.PropertyType))
                {
                    var newItem = new TabItem()
                    {
                        Id = p.Name,
                        TabName = p.GetDisplayName(),
                        Visiable = true
                    };
                    param[NamingCenter.PARAM_TYPE_NAME] = p.PropertyType.FullName;
                    param[NamingCenter.PARAM_BASE_TYPE] = p.PropertyType.FullName;
                    newItem.Content.Add(new IFrame() { Src = HttpHelper.BuildUrl(NamingCenter.CTRL_SELECT_LIST_URL, param) });
                    tab.TabItems.Add(newItem);
                }
                else
                {
                    continue;
                }
            }
            return tab;
        }

        public static Form GetFormComponent(Type entityType)
        {
            string formId = NamingCenter.GetEntityFormId(entityType);
            string key = NamingCenter.GetCacheKey(CacheType.ENTITY_CONFIG, formId);
            var result = CacheHelper.GetFromCache<Form>(key, () =>
            {
                var returnValue = new Form();
                returnValue.Id = formId;
                var pis = FastType.Get(entityType).Setters;
                int rank = 0;
                foreach (var fp in pis)
                {
                    var p = fp.Info;
                    if (!(p.PropertyType == typeof(string)) && !p.PropertyType.IsValueType)
                    {
                        continue;
                    }
                    object defaultValue = null;
                    string label = string.Empty;
                    FormControlBase fieldComponent = null;
                    var formAttr = p.GetCustomAttribute<FormFieldAttribute>(true);
                    if (formAttr != null)
                    {
                        if (!formAttr.Editable)
                        {
                            continue;
                        }
                        label = formAttr.FormDisplayName;
                        defaultValue = formAttr.DefaultValue;
                        if (formAttr.ControlType != ControlType.None)
                        {
                            switch (formAttr.ControlType)
                            {
                                case ControlType.DateTimeRange:
                                case ControlType.DatePicker:
                                    fieldComponent = new DatePicker();
                                    break;
                                case ControlType.DropDownList:
                                    fieldComponent = new DropDownList();
                                    break;
                                case ControlType.Password:
                                    fieldComponent = new TextBox() { TextMode = TextMode.Password };
                                    break;
                                case ControlType.CheckBox:
                                    fieldComponent = new CheckBox();
                                    break;
                                case ControlType.CheckBoxList:
                                    fieldComponent = new CheckBoxList();
                                    break;
                                case ControlType.RadioButton:
                                    fieldComponent = new RadioButton();
                                    break;
                                case ControlType.RadioButtonList:
                                    fieldComponent = new RadioButtonList();
                                    break;
                                case ControlType.TextArea:
                                    fieldComponent = new TextArea();
                                    break;
                                case ControlType.TextEditor:
                                    fieldComponent = new TextEditor();
                                    break;
                            }
                        }
                    }
                    if (fieldComponent == null)
                    {
                        if (p.PropertyType.IsEnum)
                        {
                            fieldComponent = new DropDownList() { DataSource = new EnumDataSource() { EnumTypeFullName = p.PropertyType.FullName, EnumValueType = EnumValueType.Code } };
                        }
                        else if (p.PropertyType == typeof(bool))
                        {
                            fieldComponent = new DropDownList() { DataSource = new EnumDataSource() { EnumTypeFullName = typeof(YesOrNo).FullName, EnumValueType = EnumValueType.Code } };
                        }
                        else if (p.PropertyType == typeof(DateTime))
                        {
                            fieldComponent = new DatePicker();
                        }
                        else
                        {
                            fieldComponent = new TextBox();
                        }
                    }
                    var formAttr1 = p.GetCustomAttribute<DataSourceAttribute>(true);
                    if (formAttr1 != null)
                    {
                        if (fieldComponent is IKeyValueDataSourceControl)
                        {
                            (fieldComponent as IKeyValueDataSourceControl).DataSource = formAttr1.GetDataSource() as IKeyValueDataSource;
                        }
                    }
                    fieldComponent.Description = p.GetDescription();
                    if (label.IsNullOrEmpty())
                    {
                        label = p.GetDisplayName();
                    }
                    var attr1 = p.GetCustomAttribute<DefaultValueAttribute>(true);
                    if (attr1 != null)
                    {
                        defaultValue = attr1.Value;
                    }
                    var attr2 = p.GetCustomAttribute<ValidatorAttribute>(true);
                    if (attr2 != null)
                    {
                        fieldComponent.Validator = attr2.ValidateString;
                    }
                    fieldComponent.Label = label;
                    fieldComponent.Id = p.Name;

                    fieldComponent.Rank = (++rank);
                    fieldComponent.Value = defaultValue == null ? "" : defaultValue.ToString();
                    returnValue.Fields.Add(fieldComponent);
                }
                foreach (var btn in ControlDefaultSetting.GetDefaultFormButton())
                {
                    returnValue.Buttons.Add(btn);
                }
                returnValue.Fields.Add(new HiddenField() { Id = ControlModelBinder.EntityFullNameHiddenName, Value = entityType.FullName + "," + entityType.Assembly.FullName });
                return returnValue;
            });
            return result.Clone() as Form;
        }
    }
}
