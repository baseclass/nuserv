namespace nuserv.Controllers
{
    #region Usings

    using System.Web.Mvc;

    #endregion

    public class HomeController : Controller
    {
        #region Public Methods and Operators

        public ActionResult About()
        {
            this.ViewBag.Message = "nuserv";

            return this.View();
        }

        public ActionResult Index()
        {
            return this.View();
        }

        #endregion
    }
}