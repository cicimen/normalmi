using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace normalmi
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SessionControlAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var skipSession = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

            if (skipSession)
            {
                if (HttpContext.Current.Session["UserInfo"] == null)
                {
                    filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.RawUrl);
                }
            }
            else
            {
                if (HttpContext.Current.Session["UserInfo"] == null)
                {
                    if (filterContext.HttpContext.Request.RawUrl == "/VisilabsManager/")
                        filterContext.Result = new RedirectResult("~/Account/Login?returnUrl=" + HttpUtility.UrlEncode("VisilabsManager/Dashboard/AnalyticsDashboard"));
                    else
                        filterContext.Result = new RedirectResult("~/Account/Login?returnUrl=" + filterContext.HttpContext.Request.RawUrl);

                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
