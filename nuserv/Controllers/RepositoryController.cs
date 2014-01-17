namespace nuserv.Controllers
{
    #region Usings

    using System;
    using System.Web.Mvc;

    #endregion

    public class RepositoryController : Controller
    {
        //
        // GET: /Repositry/

        #region Public Methods and Operators

        public ActionResult Index()
        {
            string apiUri = Url.HttpRouteUrl("DefaultApi", new { controller = "Repository", });
            ViewBag.ApiUrl = new Uri(this.Request.Url, apiUri).AbsoluteUri;

            return this.View();
        }

        #endregion
    }
}