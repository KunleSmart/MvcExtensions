using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Byaxiom.MvcExtensions.Binders
{
    public class DateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.HttpContext.Request != null)
            {
                HttpRequestBase request = controllerContext.HttpContext.Request;

                string day = request.Form[string.Format("{0}.{1}", bindingContext.ModelName, "Day")];
                string month = request.Form[string.Format("{0}.{1}", bindingContext.ModelName, "Month")];
                string year = request.Form[string.Format("{0}.{1}", bindingContext.ModelName, "Year")];

                int _month = 0;
                int.TryParse(month, out _month);

                if (_month < 1 || _month > 12) return null;
                string[] mnths = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                month = mnths[_month - 1];

                try { return DateTime.Parse(string.Format("{0} {1} {2}", day, month, year)); }
                catch { return null; }

            }
            return null;
        }

    }

}