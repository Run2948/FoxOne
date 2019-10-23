using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Threading;
using System.ComponentModel;
using FoxOne.Business;
using FoxOne.Controls;
using FoxOne.Core;
using FoxOne.Data;
using FoxOne.Data.Mapping;
namespace FoxOne.Web.Controllers
{
    public class PageDesignerController : BaseController
    {
        string pageList = "/Page/" + NamingCenter.PAGE_LIST_URL;
        string pageEdit = "/PageDesigner/PageEdit";
        string ctrlList = NamingCenter.CTRL_LIST_URL;
        string ctrlEdit = "/PageDesigner/ComponentEditor";
        string pageDesignerUrl = "/PageDesigner/PageManagement";
        string folderIconUrl = SysConfig.IconBasePath + "folder.gif";
        string appIconUrl = SysConfig.IconBasePath + "application.gif";
        string rootIconUrl = SysConfig.IconBasePath + "root.gif";
        string ctrlSelectedClass = "component-item-selected";
        string dsIconUrl = SysConfig.IconBasePath + "DataSource.gif";
        string tbIconUrl = SysConfig.IconBasePath + "list.gif";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ComponentList()
        {
            string baseTypeName = Request.QueryString[NamingCenter.PARAM_BASE_TYPE];
            string parentId = Request.QueryString[NamingCenter.PARAM_PARENT_ID];
            string ctrlId = Request.QueryString[NamingCenter.PARAM_CTRL_ID];
            string pageId = Request.QueryString[NamingCenter.PARAM_PAGE_ID];
            string targetId = Request.QueryString[NamingCenter.PARAM_TARGET_ID];
            string typeName = Request.QueryString[NamingCenter.PARAM_TYPE_NAME];
            ComponentEntity component = null;
            if (ctrlId.IsNotNullOrEmpty() && pageId.IsNotNullOrEmpty())
            {
                component = DBContext<ComponentEntity>.Instance.FirstOrDefault(o => o.PageId.Equals(pageId, StringComparison.OrdinalIgnoreCase) && o.Id.Equals(ctrlId, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                if (pageId.IsNotNullOrEmpty() && parentId.IsNotNullOrEmpty() && targetId.IsNotNullOrEmpty() && Request.QueryString[NamingCenter.PARAM_FORM_VIEW_MODE] != "Insert")
                {
                    component = DBContext<ComponentEntity>.Instance.FirstOrDefault(o =>
                        o.TargetId.Equals(targetId, StringComparison.OrdinalIgnoreCase)
                        && o.ParentId.Equals(parentId, StringComparison.OrdinalIgnoreCase)
                        && o.PageId.Equals(pageId, StringComparison.OrdinalIgnoreCase));
                }
            }
            if (component != null)
            {
                typeName = component.Type;
            }
            Type baseType = null;
            List<Type> types = null;
            if (!baseTypeName.IsNullOrEmpty())
            {
                baseType = TypeHelper.GetType(baseTypeName);
            }
            else
            {
                baseType = typeof(IControl);
            }
            if (baseType.IsInterface)
            {
                types = TypeHelper.GetAllImpl(baseType);
            }
            else
            {
                types = TypeHelper.GetAllSubType(baseType);
            }
            var models = new List<ComponentListModel>();
            string tempImage = string.Empty;
            string type = string.Empty;
            foreach (Type t in types)
            {
                if (t.IsAbstract) continue;
                tempImage = "../..{0}{1}".FormatTo(SysConfig.ControlImageBasePath, "FoxOne.Controls.Control.png");
                if (System.IO.File.Exists(Server.MapPath("~{0}{1}.png".FormatTo(SysConfig.ControlImageBasePath, t.FullName))))
                {
                    tempImage = "../../{0}{1}.png".FormatTo(SysConfig.ControlImageBasePath, t.FullName);
                }
                var attr = t.GetCustomAttribute<CategoryAttribute>(true);
                if (attr != null)
                {
                    type = attr.Category;
                    if (type.Equals("None", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }
                else
                {
                    type = (t.FullName.StartsWith("FoxOne.Controls") || t.FullName.StartsWith("FoxOne.Business")) ? "系统" : "自定义";
                }
                models.Add(new ComponentListModel()
                {
                    ComponentImage = tempImage,
                    ComponentTypeFullName = t.FullName,
                    ComponentTypeName = t.Name,
                    ComponentName = t.GetDisplayName(),
                    CssClass = (t.FullName == typeName ? ctrlSelectedClass : ""),
                    Type = type
                });
            }
            if (models.Count == 1)
            {
                models[0].CssClass = ctrlSelectedClass;
            }
            return View(models.OrderBy(o => o.ComponentName).ToList());
        }

        public JsonResult Copy(string id)
        {
            var page = PageBuilder.BuildPage(id);
            var newPage = DBContext<PageEntity>.Instance.FirstOrDefault(o => o.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            newPage.Id += "_Copy";
            newPage.Title += "_副本";
            DBContext<PageEntity>.Insert(newPage);
            foreach (var item in page.Children)
            {
                item.PageId = newPage.Id;
                item.ParentId = newPage.Id;
                ComponentHelper.RecSave(item);
            }
            return Json(true);
        }

        public ActionResult TableList()
        {
            return View();
        }

        public ActionResult ControlList()
        {
            var table = new FoxOne.Controls.Table()
            {
                ShowIndex = true,
                ShowCheckBox = true,
                AllowPaging = false,
                AutoGenerateColum = false,
                AutoHeight = true
            };
            table.KeyFieldName = "Id";
            table.Columns.Add(new TableColumn() { FieldName = "Id" });
            table.Columns.Add(new TableColumn() { FieldName = "Type" });
            table.Columns.Add(new TableColumn() { FieldName = "Rank" });
            table.Columns.Add(new TableColumn() { FieldName = "TargetId" });
            table.Columns.Add(new TableColumn() { FieldName = "LastUpdateTime" });
            table.Buttons.Add(new TableButton() { CssClass = "btn btn-primary btn-sm", Id = "btnEdit", Name = "编辑", OnClick = "editCtrl('{0}','{1}')", DataFields = "PageId,Id" });
            table.Buttons.Add(new TableButton() { CssClass = "btn btn-danger btn-sm", Id = "btnDelete", Name = "删除", OnClick = "deleteCtrl('{0}','{1}')", DataFields = "PageId,Id" });
            var ds = new EntityDataSource() { EntityType = typeof(ComponentEntity) };
            ds.DataFilter = new RequestParameterDataFilter() { ParameterRange = ParameterRange.QueryString };
            table.DataSource = ds;
            ViewData["table"] = table;
            return View();
        }

        public ActionResult PageEdit()
        {
            PageEntity model = new PageEntity();
            string pageId = Request.QueryString[NamingCenter.PARAM_KEY_NAME];
            if (pageId.IsNotNullOrEmpty())
            {
                ViewData[NamingCenter.PARAM_FORM_VIEW_MODE] = "Edit";
                model = DBContext<PageEntity>.Instance.FirstOrDefault(o => o.Id.Equals(pageId, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                ViewData[NamingCenter.PARAM_FORM_VIEW_MODE] = "Insert";
            }
            var parentId = new List<SelectListItem>();
            parentId.Add(new SelectListItem() { Text = "请选择", Value = "Root" });
            foreach (var item in DBContext<PageEntity>.Instance.Where(o => o.Type.Equals("Module")))
            {
                parentId.Add(new SelectListItem() { Text = item.Title, Value = item.Id, Selected = item.Id.Equals(model.ParentId) });
            }
            ViewData["ParentId"] = parentId;

            var layoutId = new List<SelectListItem>();
            foreach (var item in DBContext<LayoutEntity>.Instance.Where(o => true))
            {
                layoutId.Add(new SelectListItem() { Text = item.Title, Value = item.Id, Selected = item.Id.Equals(model.LayoutId) });
            }
            ViewData["LayoutId"] = layoutId;

            var type = new List<SelectListItem>();
            type.Add(new SelectListItem() { Text = "模块", Value = "Module", Selected = "Module".Equals(model.Type) });
            type.Add(new SelectListItem() { Text = "页面", Value = "Page", Selected = "Page".Equals(model.Type) });
            ViewData["Type"] = type;

            var service = new List<SelectListItem>();
            foreach (var item in TypeHelper.GetAllImpl<IPageService>())
            {
                service.Add(new SelectListItem() { Text = item.FullName, Value = item.FullName, Selected = item.FullName.Equals(model.Service) });
            }
            ViewData["Service"] = service;
            return View(model);
        }

        [HttpPost]
        public JsonResult PageEdit(PageEntity entity)
        {
            var isInsert = Request.Form[NamingCenter.PARAM_FORM_VIEW_MODE].Equals("Insert");
            bool result = false;
            if (isInsert)
            {
                if (DBContext<PageEntity>.Instance.FirstOrDefault(o => o.Id.Equals(entity.Id, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    throw new FoxOneException("Page_Id_Exist");
                }
                result = DBContext<PageEntity>.Insert(entity);
            }
            else
            {
                result = DBContext<PageEntity>.Update(entity);
            }
            return Json(result);
        }

        public JsonResult ClearTableCache()
        {
            TableMapper.RefreshTableCache(Dao.Get());
            return Json(true);
        }

        public ActionResult CRUDEdit()
        {
            string crudId = Request.QueryString[NamingCenter.PARAM_KEY_NAME];
            string reGenerate = Request.QueryString["ReGenerate"];
            string id = Request.QueryString["Id"];
            var model = new CRUDEntity();
            if (reGenerate.IsNotNullOrEmpty() && reGenerate.Equals("true"))
            {
                if (crudId.IsNotNullOrEmpty())
                {
                    ViewData[NamingCenter.PARAM_FORM_VIEW_MODE] = Request.QueryString[NamingCenter.PARAM_FORM_VIEW_MODE];
                    var pageGenerator = new PageGenerator() { TableName = crudId };
                    model = pageGenerator.GetCRUDEntity();
                    model.Id = id;
                }
            }
            else
            {
                if (crudId.IsNotNullOrEmpty())
                {
                    ViewData[NamingCenter.PARAM_FORM_VIEW_MODE] = "Edit";
                    model = DBContext<CRUDEntity>.Instance.FirstOrDefault(o => o.Id.Equals(crudId, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    ViewData[NamingCenter.PARAM_FORM_VIEW_MODE] = "Insert";
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CRUDEdit(CRUDEntity entity)
        {
            var isInsert = Request.Form[NamingCenter.PARAM_FORM_VIEW_MODE].Equals("Insert");
            bool result = false;
            if (isInsert)
            {
                if (DBContext<CRUDEntity>.Instance.FirstOrDefault(o => o.Id.Equals(entity.Id, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    throw new FoxOneException("Crud_Id_Exist");
                }
                result = DBContext<CRUDEntity>.Insert(entity);
            }
            else
            {
                result = DBContext<CRUDEntity>.Update(entity);
            }
            return Json(result);
        }

        public JsonResult ComponentDelete()
        {
            string ctrlId = Request.Form[NamingCenter.PARAM_CTRL_ID];
            string pageId = Request.Form[NamingCenter.PARAM_PAGE_ID];
            ComponentHelper.DeleteComponent(pageId, ctrlId);
            return Json(true);
        }

        public class TableDataSource : ListDataSourceBase
        {
            FoxOne.Data.Mapping.Table _table;
            public TableDataSource(FoxOne.Data.Mapping.Table table)
            {
                _table = table;
            }
            public override IEnumerable<IDictionary<string, object>> GetList()
            {
                return _table.Columns.ToDictionary();
            }
        }

        public JsonResult GetTable(string id)
        {
            var tables = FoxOne.Data.Mapping.TableMapper.Tables[Dao.Get().ConnectionString];
            if (id.IsNotNullOrEmpty())
            {
                var dataTable = tables.FirstOrDefault(o => o.Name.Equals(id, StringComparison.OrdinalIgnoreCase));
                var table = new FoxOne.Controls.Table() { AllowPaging = false, ShowCheckBox = false, ShowIndex = false, AutoGenerateColum = false };
                table.Columns.Add(new TableColumn() { FieldName = "Name", ColumnName = "字段名", TextAlign = CellTextAlign.Left });
                table.Columns.Add(new TableColumn() { FieldName = "Comment", ColumnName = "注释", TextAlign = CellTextAlign.Left });
                table.Columns.Add(new TableColumn() { FieldName = "Type", ColumnName = "类型" });
                table.Columns.Add(new TableColumn() { FieldName = "Length", ColumnName = "长度" });
                table.Columns.Add(new TableColumn() { FieldName = "IsKey", ColumnName = "主键" });
                table.Columns.Add(new TableColumn() { FieldName = "IsNullable", ColumnName = "可为空" });
                table.Columns.Add(new TableColumn() { FieldName = "IsAutoIncrement", ColumnName = "自增" });
                table.DataSource = new TableDataSource(dataTable);
                return Json(table.Render());
            }
            else
            {
                var result = new List<TreeNode>();
                result.Add(new TreeNode() { Value = "Root", Text = tables[0].Schema, Open = true, ParentId = "", Icon = dsIconUrl });
                foreach (var table in tables)
                {
                    result.Add(new TreeNode() { Value = table.Name, Icon = tbIconUrl, Open = false, Text = table.Name, ParentId = "Root" });
                }
                return Json(result);
            }
        }

        public JsonResult GetPage()
        {
            var result = DBContext<PageEntity>.Instance.Where(o => true);
            var permissions = new List<TreeNode>();
            var param = new Dictionary<string, object>();
            bool isOpen = false;
            foreach (var r in result)
            {
                param[NamingCenter.PARAM_PAGE_ID] = r.Id;
                param[NamingCenter.PARAM_TYPE_NAME] = r.Type;
                isOpen = r.Type.Equals(PermissionType.Module.ToString());
                permissions.Add(new TreeNode()
                {
                    Value = r.Id,
                    ParentId = r.ParentId.IsNullOrEmpty() ? "Root" : r.ParentId,
                    Text = r.Title,
                    Open = isOpen,
                    Url = HttpHelper.BuildUrl(pageDesignerUrl, param),
                    Icon = isOpen ? folderIconUrl : appIconUrl
                });
            }
            permissions.Add(new TreeNode()
            {
                Value = "Root",
                ParentId = "null",
                Text = ObjectHelper.GetObject<ILangProvider>().GetString("AllPage"),
                Open = true,
                Icon = rootIconUrl,
                Url = pageDesignerUrl
            });
            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        private class InnerTabModel { public string Id { get; set; } public string Url { get; set; } }

        public ActionResult Pagemanagement()
        {
            string pageId = Request.QueryString[NamingCenter.PARAM_PAGE_ID];
            string type = Request.QueryString[NamingCenter.PARAM_TYPE_NAME];
            var param = new Dictionary<string, object>();
            var tabList = new List<InnerTabModel>();
            var tab = new Tab() { Id = "PageTab", PageId = "PageManagement", InitIndex = 0 };
            if (type.IsNotNullOrEmpty())
            {
                param.Add(NamingCenter.PARAM_KEY_NAME, pageId);
                tabList.Add(new InnerTabModel() { Id = "PageInfo", Url = HttpHelper.BuildUrl(pageEdit, param) });
                param.Clear();
            }
            if (pageId.IsNullOrEmpty())
            {
                pageId = "Root";
            }
            param.Add(NamingCenter.PARAM_PARENT_ID, pageId);
            param.Add(NamingCenter.PARAM_PAGE_ID, pageId);
            if (type == PermissionType.Page.ToString())
            {
                tabList.Add(new InnerTabModel() { Id = "ComponentInfo", Url = HttpHelper.BuildUrl(ctrlList, param) });
                tabList.Add(new InnerTabModel() { Id = "ExtFileInfo", Url = HttpHelper.BuildUrl("/Page/PageLayoutFileEntityList", param) });
                tabList.Add(new InnerTabModel() { Id = "Design", Url = "/Page/PreView?" + NamingCenter.PARAM_PAGE_ID + "=" + pageId });
                tabList.Add(new InnerTabModel() { Id = "PreView", Url = "/Page/" + pageId });
                tabList.Add(new InnerTabModel() { Id = "Other", Url = "/PageDesigner/Other/" + pageId });
            }
            else
            {
                tabList.Add(new InnerTabModel() { Id = "ChildrenInfo", Url = HttpHelper.BuildUrl(pageList, param) });
            }
            tabList.ForEach(o =>
            {
                var tabItem = new TabItem()
                {
                    Id = o.Id,
                    Visiable = true,
                    TabName = ObjectHelper.GetObject<ILangProvider>().GetString(o.Id)
                };
                tabItem.Content.Add(new IFrame() { Src = o.Url, Id = "{0}-Iframe".FormatTo(o.Id) });
                tab.TabItems.Add(tabItem);
            });
            return View(tab);
        }

        public ActionResult Other(string id)
        {
            var page = DBContext<PageEntity>.Instance.FirstOrDefault(o => o.Id.Equals(id));
            ViewData["page"] = page;
            return View();
        }

        public ActionResult ComponentEditor()
        {
            var model = new FormModel();
            string ctrlId = Request.QueryString[NamingCenter.PARAM_CTRL_ID];
            string typeName = Request.QueryString[NamingCenter.PARAM_TYPE_NAME];
            string pageId = Request.QueryString[NamingCenter.PARAM_PAGE_ID];
            string parentId = Request.QueryString[NamingCenter.PARAM_PARENT_ID];
            string targetId = Request.QueryString[NamingCenter.PARAM_TARGET_ID];
            Type type = null;
            IControl instance = null;
            ComponentEntity component = null;
            if (ctrlId.IsNullOrEmpty())
            {
                if (!pageId.IsNullOrEmpty() && !parentId.IsNullOrEmpty() && !targetId.IsNullOrEmpty() && Request.QueryString[NamingCenter.PARAM_FORM_VIEW_MODE] != "Insert")
                {
                    component = DBContext<ComponentEntity>.Instance.FirstOrDefault(o => o.PageId.Equals(pageId, StringComparison.OrdinalIgnoreCase) && o.TargetId.Equals(targetId, StringComparison.OrdinalIgnoreCase) && o.ParentId.Equals(parentId, StringComparison.OrdinalIgnoreCase));
                }
            }
            else
            {
                component = DBContext<ComponentEntity>.Instance.FirstOrDefault(o => o.PageId.Equals(pageId, StringComparison.OrdinalIgnoreCase) && o.Id.Equals(ctrlId, StringComparison.OrdinalIgnoreCase));
            }
            if (component != null)
            {
                ctrlId = component.Id;
                pageId = component.PageId;
                parentId = component.ParentId;
                if (typeName.IsNullOrEmpty() || typeName.Equals(component.Type))
                {
                    type = TypeHelper.GetType(component.Type);
                    if (!component.JsonContent.IsNullOrEmpty())
                    {
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        serializer.RegisterConverters(new[] { new FoxOne.Business.ComponentConverter() });
                        instance = serializer.Deserialize(component.JsonContent, type) as IControl;
                        if (instance == null)
                        {
                            throw new FoxOneException("Ctrl_Not_Valid");
                        }
                        instance.Id = component.Id;
                    }
                    typeName = type.FullName;
                    model.Tab = ComponentHelper.GetTabComponent(instance, ctrlId, pageId);
                }
            }
            if (typeName.IsNullOrEmpty())
            {
                throw new FoxOneException("Parameter_Not_Found", NamingCenter.PARAM_TYPE_NAME + " Or " + NamingCenter.PARAM_CTRL_ID);
            }
            type = TypeHelper.GetType(typeName);
            if (parentId.IsNullOrEmpty())
            {
                parentId = pageId;
            }
            model.EntityName = type.FullName;
            model.Form = ComponentHelper.GetFormComponent(type);
            model.Form.PostUrl = HttpHelper.BuildUrl(ctrlEdit, Request);

            if (instance == null)
            {
                model.Form.FormMode = FormMode.Insert;
                instance = Activator.CreateInstance(type) as IControl;
                if (ctrlId.IsNotNullOrEmpty())
                {
                    instance.Id = ctrlId;
                    model.Form.FormMode = FormMode.Edit;
                }
                else
                {
                    if (!pageId.IsNullOrEmpty())
                    {
                        instance.Id = pageId + type.Name;
                    }
                }
            }
            else
            {
                model.Form.FormMode = FormMode.Edit;
            }

            //如果是编辑控件，则控件ID不允许更改。
            //model.Form.Fields.FirstOrDefault(o => o.Id.Equals("Id", StringComparison.OrdinalIgnoreCase)).Enable = !(model.Form.FormMode == FormMode.Edit);

            model.Form.Data = instance;
            if (model.Tab != null)
            {
                if (model.Tab.TabItems.Count == 1)
                {
                    model.Tab = null;
                }
                else
                {
                    model.Tab.TabItems[0].Content.Add(model.Form);
                }
            }
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult ComponentEditor(IControl entity)
        {
            bool isInsert = !Request[NamingCenter.PARAM_FORM_VIEW_MODE].Equals(FormMode.Edit.ToString(), StringComparison.OrdinalIgnoreCase);
            entity.PageId = Request.QueryString[NamingCenter.PARAM_PAGE_ID];
            entity.ParentId = Request.QueryString[NamingCenter.PARAM_PARENT_ID];
            if (entity.ParentId.IsNullOrEmpty())
            {
                entity.ParentId = entity.PageId;
            }
            entity.TargetId = Request.QueryString[NamingCenter.PARAM_TARGET_ID];
            if (entity.TargetId.IsNullOrEmpty())
            {
                entity.TargetId = "Right";
            }
            ComponentHelper.SaveComponent(entity, isInsert);
            return Json(entity.Id);
        }

        public JsonResult MergeTableColumn()
        {
            var ctrlIds = Request[NamingCenter.PARAM_CTRL_ID].Split(',');
            var pageId = Request[NamingCenter.PARAM_PAGE_ID];
            var mergeColumns = DBContext<ComponentEntity>.Instance.Where(o => o.PageId.Equals(pageId, StringComparison.OrdinalIgnoreCase) && ctrlIds.Contains(o.Id, StringComparer.OrdinalIgnoreCase)).OrderBy(o => o.Rank);
            var c = mergeColumns.FirstOrDefault();
            int i = 1;
            string newColumnId = "columnMerge{0}";
            var columns = DBContext<ComponentEntity>.Instance.Where(o => o.ParentId.Equals(c.ParentId, StringComparison.CurrentCultureIgnoreCase) && o.TargetId.Equals(c.TargetId, StringComparison.CurrentCultureIgnoreCase));
            while (columns.Count(o => o.Id.Equals(newColumnId.FormatTo(i), StringComparison.CurrentCultureIgnoreCase)) > 0)
            {
                i++;
            }
            var tempColumn = new TableColumn()
            {
                ColumnName = "合并列{0}".FormatTo(i),
                FieldName = "MergeColumn{0}".FormatTo(i),
                Id = newColumnId.FormatTo(i),
                Visiable = true,
                Rank = i,
                PageId = c.PageId,
                ParentId = c.ParentId,
                TargetId = c.TargetId
            };
            ComponentHelper.SaveComponent(tempColumn);
            int j = 1;
            foreach (var m in mergeColumns)
            {
                m.ParentId = tempColumn.Id;
                m.TargetId = "Children";
                m.Rank = i * 10 + (j++);
                DBContext<ComponentEntity>.Update(m);
            }
            return Json(true);
        }

        public JsonResult GetFormField()
        {
            string id = Request.Form[NamingCenter.PARAM_PARENT_ID];
            string pageId = Request.Form[NamingCenter.PARAM_PAGE_ID];
            var component = PageBuilder.BuildPage(pageId).FindControl(id);
            if (component is Form)
            {
                var form = component as Form;
                if (form.FormService == null)
                {
                    throw new FoxOneException("Need_DataSource", form.Id);
                }
                if (form.FormService is EntityDataSource)
                {
                    var tempForm = ComponentHelper.GetFormComponent((form.FormService as EntityDataSource).EntityType);
                    foreach (var field in tempForm.Fields)
                    {
                        field.PageId = pageId;
                        field.ParentId = id;
                        field.TargetId = "Fields";
                        ComponentHelper.RecSave(field);
                    }
                    return Json(true);
                }
                if (form.FormService is DataTableDataSource)
                {
                    var tableName = (form.FormService as DataTableDataSource).TableName;
                    var table = FoxOne.Data.Mapping.TableMapper.Tables[Dao.Get().ConnectionString].FirstOrDefault(o => o.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
                    foreach (var f in table.Columns.Where(o => o.Editable).OrderBy(o => o.Rank))
                    {
                        var field = ControlDefaultSetting.GetFormControl(f);
                        field.PageId = pageId;
                        field.ParentId = id;
                        field.TargetId = "Fields";
                        ComponentHelper.RecSave(field);
                    }
                    return Json(true);
                }
            }
            throw new FoxOneException("Only_Suppost_Form_Ctrl");
        }

        public JsonResult GetTableColumn()
        {
            string id = Request.Form[NamingCenter.PARAM_PARENT_ID];
            string pageId = Request.Form[NamingCenter.PARAM_PAGE_ID];
            var component = PageBuilder.BuildPage(pageId).FindControl(id);
            if (component is FoxOne.Controls.Table)
            {
                var table = component as FoxOne.Controls.Table;
                if (table.DataSource == null)
                {
                    throw new FoxOneException("Need_DataSource", table.Id);
                }
                var data = table.GetData();
                if (data.IsNullOrEmpty())
                {
                    throw new FoxOneException("DataSource_Return_Empty", table.Id);
                }
                table.GenerateTableColumn(data);
                foreach (var column in table.Columns)
                {
                    column.PageId = pageId;
                    column.ParentId = id;
                    column.TargetId = "Columns";
                    ComponentHelper.RecSave(column);
                }
                return Json(true);
            }
            throw new FoxOneException("Only_Suppost_Table_Ctrl");
        }
        public JsonResult CleanCache(string id)
        {
            return Json(CacheHelper.Remove(NamingCenter.GetCacheKey(CacheType.PAGE_CONFIG, id)));
        }
    }
}
