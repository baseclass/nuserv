namespace nuserv.Utility
{
    #region Usings

    using System;
    using System.Web.Http.Routing;

    #endregion

    public interface IHttpRouteDataResolver
    {
        #region Public Methods and Operators

        IHttpRouteData Resolve();

        #endregion
    }
}