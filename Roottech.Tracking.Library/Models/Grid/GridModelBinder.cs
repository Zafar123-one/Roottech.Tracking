using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Roottech.Tracking.Library.Models.Grid
{
    public class GridModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            try
            {
                var request = System.Web.HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);
                bindingContext.Model = new GridSettings
                {
                    IsSearch = bool.Parse(request["_search"] ?? "false"),
                    PageIndex = int.Parse(request["page"] ?? "1"),
                    PageSize = int.Parse(request["rows"] ?? "10"),
                    SortColumn = request["sidx"] ?? "",
                    SortOrder = request["sord"] ?? "asc",
                    Where = Filter.Create(request["filters"] ?? "")
                };
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}