namespace nuserv.Service.Contracts
{
    #region Usings

    using System.Linq;

    using Lucene.Net.Linq;

    using nuserv.Models;
    using nuserv.Models.Contracts;

    #endregion

    public interface IRepositoryRepository
    {
        #region Public Methods and Operators

        ISession<Repository> OpenSession();

        IQueryable<Repository> AsQueryable();

        #endregion
    }
}