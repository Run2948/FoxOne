
using FoxOne.Business.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FoxOne.Business
{
    public interface ISortableControl : IControl
    {
        int Rank { get; set; }
    }

    public interface IComponent : ISortableControl
    {
        string CssClass { get; set; }

        bool Visiable { get; set; }

        string Render();
    }

    public interface IListDataSourceControl
    {
        IListDataSource DataSource { get; set; }
    }

    public interface ICascadeDataSourceControl
    {
        ICascadeDataSource DataSource { get; set; }
    }

    public interface IPageService
    {

    }


}
