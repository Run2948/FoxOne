using FoxOne.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoxOne.Core;
using FoxOne.Business;
using FoxOne.Data.Mapping;
using FoxOne.Data;
namespace FoxOne.Web
{
    public class PageGenerator
    {
        public FoxOne.Data.Mapping.Table Table
        {
            get
            {
                FoxOne.Data.Mapping.Table result = null;
                if (Type != null)
                {
                    result = TableMapper.ReadTable(Type);
                }
                else
                {
                    if (TableName.IsNotNullOrEmpty())
                    {
                        result = TableMapper.Tables[Dao.Get().ConnectionString].FirstOrDefault(o => o.Name.Equals(TableName, StringComparison.OrdinalIgnoreCase));
                    }
                }
                return result;
            }
        }

        public string TableName
        {
            get;
            set;
        }

        public Type Type
        {
            get;
            set;
        }

        public string PageTitle
        {
            get;
            set;
        }

        public string ListPageName
        {
            get;
            set;
        }

        public string EditPageName
        {
            get;
            set;
        }

        public string CRUDName
        {
            get;
            set;
        }

        public string ParentId
        {
            get
            {
                string result = string.Empty;
                if (ParentName.IsNullOrEmpty())
                {
                    ParentName = "系统管理";
                }
                var pageEntity = DBContext<PageEntity>.Instance.FirstOrDefault(o => o.Title.Equals(ParentName, StringComparison.OrdinalIgnoreCase));
                if (pageEntity != null)
                {
                    result = pageEntity.Id;
                }
                else
                {
                    result = ParentName.GetPY();
                    DBContext<PageEntity>.Insert(new PageEntity() { Id = result, Title = ParentName, LayoutId = "", Type = PermissionType.Module.ToString(), RentId = RentId, LastUpdateTime = DateTime.Now, CssClass = "" });
                    DBContext<IPermission>.Insert(new Permission() { Id = result, Name = ParentName, Code = result, Url = "", Rank = 1, RentId = RentId, Type = PermissionType.Module, LastUpdateTime = DateTime.Now, Status = "Enabled" });
                }
                return result;
            }
        }

        public string ParentName
        {
            get;
            set;
        }

        public int RentId
        {
            get
            {
                return 1;
            }
        }


        public void AddEditPage()
        {
            var pageEntity = new PageEntity() { Id = EditPageName, ParentId = ParentId, Title = PageTitle + "编辑", LayoutId = "Normal", Type = PermissionType.Page.ToString(), RentId = RentId, LastUpdateTime = DateTime.Now, CssClass = "easyui-layout" };
            DBContext<PageEntity>.Insert(pageEntity);
            //AddFunction(pageEntity.Title, pageEntity.Id, "/Page/" + EditPageName);
            AddForm();
        }

        public void AddListPage()
        {
            var pageEntity = new PageEntity() { Id = ListPageName, ParentId = ParentId, Title = PageTitle + "列表", LayoutId = "Normal", Type = PermissionType.Page.ToString(), RentId = RentId, LastUpdateTime = DateTime.Now, CssClass = "easyui-layout" };
            DBContext<PageEntity>.Insert(pageEntity);
            //AddFunction(pageEntity.Title, pageEntity.Id, "/Page/" + ListPageName);
            AddSearch();
            AddToolbar();
            AddTable();
        }

        private void AddFunction(string title, string pageId, string url)
        {
            string permissionId = Guid.NewGuid().ToString();
            DBContext<IPermission>.Insert(new Permission() { Id = permissionId, Name = title, ParentId = ParentId, Code = pageId, Url = url, Rank = 1, RentId = RentId, Type = PermissionType.Page, Status = "Enabled", LastUpdateTime = DateTime.Now });
            string defaultRoleId = DBContext<Role>.Instance.FirstOrDefault(o => o.RoleType.Name.Equals("系统管理员")).Id;
            DBContext<IRolePermission>.Insert(new RolePermission() { Id = Guid.NewGuid().ToString(), RoleId = defaultRoleId, PermissionId = permissionId, RentId = RentId });
        }



        private void AddTable()
        {
            if (Table == null) return;
            var table = new FoxOne.Controls.Table() { ShowCheckBox = true, ShowIndex = true, Rank = 2, Id = tableListTemplate.FormatTo(ListPageName), PageId = ListPageName, ParentId = ListPageName, TargetId = "Right" };
            if (Type != null)
            {
                table.DataSource = new EntityDataSource() { EntityTypeFullName = Type.FullName };
            }
            else
            {
                table.DataSource = new CRUDDataSource() { CRUDName = this.CRUDName };
            }
            var key = Table.Columns.FirstOrDefault(o => o.IsKey);
            table.KeyFieldName = key == null ? "Id" : key.Name;
            table.EditUrl = "/Page/" + EditPageName;
            table.InsertUrl = table.EditUrl;
            table.DeleteUrl = "/Entity/Delete";
            foreach (var c in Table.Columns)
            {
                if (c.Showable)
                {
                    table.Columns.Add(new TableColumn() { Rank = c.Rank, FieldName = c.Name, Id = "column{0}".FormatTo(c.Name), ColumnName = c.Comment });
                }
            }
            foreach (var btn in ControlDefaultSetting.GetDefaultTableButton())
            {
                table.Buttons.Add(btn);
            }
            ComponentHelper.RecSave(table);
        }
        string tableListTemplate = "{0}Table";
        string toolbarTemplate = "{0}Toolbar";
        private void AddToolbar()
        {
            if (Table == null) return;
            var toolbar = new Toolbar() { Rank = 1, Id = toolbarTemplate.FormatTo(ListPageName), PageId = ListPageName, ParentId = ListPageName, TargetId = "Right", TargetControlId = tableListTemplate.FormatTo(ListPageName) };
            foreach (var btn in ControlDefaultSetting.GetDefaultToolbarButton())
            {
                toolbar.Buttons.Add(btn);
            }
            ComponentHelper.RecSave(toolbar);
        }

        private void AddSearch()
        {
            if (Table == null) return;
            var search = new Search() { Rank = 2, Id = "{0}Search".FormatTo(ListPageName), PageId = ListPageName, ParentId = ListPageName, TargetId = "Right", TargetControlId = tableListTemplate.FormatTo(ListPageName) };
            foreach (var field in Table.Columns.Where(o => o.Searchable).OrderBy(o => o.Rank))
            {
                var f = ControlDefaultSetting.GetFormControl(field);
                f.Id = field.Name;
                f.Name = field.Name;
                f.Label = field.Comment;
                f.Rank = field.Rank;
                f.Enable = true;
                f.Visiable = true;
                search.Fields.Add(f);
            }
            foreach (var btn in ControlDefaultSetting.GetDefaultSearchButton())
            {
                search.Buttons.Add(btn);
            }
            ComponentHelper.RecSave(search);
        }

        private void AddForm()
        {
            Form form = null;
            if (Type != null)
            {
                form = ComponentHelper.GetFormComponent(Type);
            }
            else
            {
                if (Table == null) return;
                form = new Form() { Rank = 1, Id = "{0}Form".FormatTo(EditPageName) };
                form.PostUrl = "/Entity/Edit";
                form.FormMode = FormMode.Insert;
                form.AppendQueryString = true;
                form.FormService = new CRUDDataSource() { CRUDName = CRUDName, Id = form.Id + "Ds" };
                foreach (var field in Table.Columns.Where(o => o.Editable).OrderBy(o => o.Rank))
                {
                    var f = ControlDefaultSetting.GetFormControl(field);
                    form.Fields.Add(f);
                }
                foreach (var btn in ControlDefaultSetting.GetDefaultFormButton())
                {
                    form.Buttons.Add(btn);
                }
            }
            form.PageId = EditPageName;
            form.ParentId = EditPageName;
            form.TargetId = "Right";
            ComponentHelper.RecSave(form);
        }

        public void AddCRUD()
        {
            if (DBContext<CRUDEntity>.Instance.Where(o => o.Id.Equals(CRUDName, StringComparison.OrdinalIgnoreCase)).Count() > 0)
            {
                return;
            }
            var entity = GetCRUDEntity();
            DBContext<CRUDEntity>.Insert(entity);
        }

        public CRUDEntity GetCRUDEntity()
        {
            var mapping = new TableMapping(Table);
            var provider = Dao.Get().MappingProvider;
            string pkName = "";
            string parentField = "";
            string titleField = "";
            string sortField = "";
            var key = mapping.Table.Keys.FirstOrDefault();
            if (key != null)
            {
                pkName = key.Name;
            }
            else
            {
                key = mapping.Table.Columns.FirstOrDefault(o => o.Name == "Id");
                if (key != null)
                {
                    pkName = key.Name;
                }
            }
            var key1 = mapping.Table.Columns.FirstOrDefault(o => o.Name.IndexOf("Parent") >= 0);
            if (key1 != null)
            {
                parentField = key1.Name;
            }
            var key2 = mapping.Table.Columns.FirstOrDefault(o => o.Name.Equals("Name"));
            if (key2 != null)
            {
                titleField = key2.Name;
            }
            var key3 = mapping.Table.Columns.FirstOrDefault(o => o.Name.Equals("Rank"));
            if (key3 != null)
            {
                sortField = key3.Name;
            }
            else
            {
                sortField = pkName;
            }
            var entity = new CRUDEntity()
            {
                Id = CRUDName,
                PKName = pkName,
                RentId = RentId,
                SelectSQL = provider.CreateSelectStatement(mapping),
                GetOneSQL = provider.CreateGetOneStatement(mapping),
                InsertSQL = provider.CreateInsertStatement(mapping),
                UpdateSQL = provider.CreateUpdateStatement(mapping),
                DeleteSQL = provider.CreateDeleteStatement(mapping),
                LastUpdateTime = DateTime.Now,
                TableName = mapping.Table.Name,
                ValueField = pkName,
                TitleField = titleField,
                DefaultSortExpression = sortField,
                ParentField = parentField
            };
            return entity;
        }
    }
}