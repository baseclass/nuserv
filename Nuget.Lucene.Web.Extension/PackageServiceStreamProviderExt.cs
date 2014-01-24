namespace NuGet.Lucene.Web.Extension
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Routing;

    using NuGet.Lucene.Web.DataServices;

    #endregion

    public class PackageServiceStreamProviderExt : PackageServiceStreamProvider
    {
        #region Fields

        private readonly Regex placeholderRegex = new Regex(@"\{([^\}]*)}");

        private string[] packagesDownloadRoutePlaceholders = null;

        #endregion

        #region Public Methods and Operators

        public override string GetPackageDownloadPath(DataServicePackage package)
        {
            var route = RouteTable.Routes[RouteNames.Packages.Download];

            var routeValues = new { id = package.Id, version = package.Version, httproute = true };

            var routeValueDictionary = new RouteValueDictionary(routeValues);

            this.AddMissingRouteDataFromCurrentRequest(routeValueDictionary);

            var virtualPathData = route.GetVirtualPath(RequestContext, routeValueDictionary);
            if (virtualPathData != null)
            {
                var path = virtualPathData.VirtualPath;
                return VirtualPathUtility.ToAbsolute("~/" + path);
            }

            throw new InvalidOperationException("Can't calculate valid route for package!");
        }

        #endregion

        #region Methods

        private void AddMissingRouteDataFromCurrentRequest(RouteValueDictionary routeValueDictionary)
        {
            var webRoute = RouteTable.Routes[RouteNames.Packages.Download] as Route;

            if (webRoute != null)
            {
                if (this.packagesDownloadRoutePlaceholders == null)
                {
                    this.packagesDownloadRoutePlaceholders = this.GetPlaceholderKeys(webRoute.Url).ToArray();
                }

                var missingKeys = this.packagesDownloadRoutePlaceholders.Where(
                    k => !routeValueDictionary.ContainsKey(k));

                var serviceRouteData = HttpContext.Current.Request.RequestContext.RouteData;

                foreach (
                    var missingKey in missingKeys.Where(missingKey => serviceRouteData.Values.ContainsKey(missingKey)))
                {
                    routeValueDictionary.Add(missingKey, serviceRouteData.Values[missingKey]);
                }
            }
        }

        private IEnumerable<string> GetPlaceholderKeys(string url)
        {
            return this.placeholderRegex.Matches(url).Cast<Match>().Select(m => m.Groups[1].Value);
        }

        #endregion
    }
}