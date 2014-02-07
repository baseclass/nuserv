namespace nuserv.Controllers
{
    #region Usings

    using System.Web.Mvc;

    #endregion

    public class RepositoryController : Controller
    {
        //
        // GET: /Repositry/

        #region Public Methods and Operators

        public ActionResult Index()
        {
            return this.View();
        }

        #endregion
    }
}