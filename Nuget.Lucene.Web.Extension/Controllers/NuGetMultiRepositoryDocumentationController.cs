using AspNet.WebApi.HtmlMicrodataFormatter;
using NuGet.Lucene.Web.Controllers;

namespace NuGet.Lucene.Web.Extension.Controllers
{
    #region Usings

    #endregion

    /// <summary>
    ///     Provides documentation and semantic information about various
    ///     resources and actions configured for use in this application.
    /// </summary>
    public class NuGetMultiRepositoryDocumentationController : DocumentationController
    {
        #region Public Properties

        public NuGetMultiRepositoryWebApiRouteMapper NuGetWebApiRouteMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Probably the document you are reading now.
        /// </summary>
        public override SimpleApiDocumentation GetApiDocumentation()
        {
            var docs = base.GetApiDocumentation();

            docs.Add(
                "Packages",
                new SimpleApiDescription(Request, "OData", NuGetWebApiRouteMapper.ODataRoutePath)
                {
                    Documentation = DocumentationProvider.GetDocumentation(typeof(PackagesODataController))
                });

            return docs;
        }

        #endregion
    }
}