using System.Web.Mvc;

namespace Byaxiom.MvcExtensions
{
    public class CookieTempControllerBase : Controller
    {
        protected override ITempDataProvider CreateTempDataProvider()
        {
            return new CookieTempDataProvider(HttpContext);
        }
    }
}
