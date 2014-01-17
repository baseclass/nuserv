namespace NuGet.Lucene.Web.Extension.Controllers
{
    #region Usings

    using AspNet.WebApi.HtmlMicrodataFormatter;

    using NuGet.Lucene.Web.DataServices;
    using NuGet.Lucene.Web.Hubs;

    #endregion

    /// <summary>
    ///     Provides documentation and semantic information about various
    ///     resources and actions configured for use in this application.
    /// </summary>
    public class NuGetMultiRepositoryDocumentationController : DocumentationController
    {
        #region Public Properties

        [Ninject.Inject]
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
                new SimpleApiDescription(this.Request, "OData", this.NuGetWebApiRouteMapper.ODataRoutePath)
                {
                    Documentation
                        =
                        this
                        .DocumentationProvider
                        .GetDocumentation
                        (
                            typeof
                        (
                        PackageDataService
                        ))
                });
            
            return docs;
        }

        #endregion
    }
}