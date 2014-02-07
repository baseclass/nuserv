namespace nuserv.Service.Contracts
{
    #region Usings

    using System.Linq;

    using Lucene.Net.Linq;

    using nuserv.Models;

    #endregion

    public interface IRepositoryRepository
    {
        #region Public Methods and Operators

        IQueryable<Repository> AsQueryable();

        ISession<Repository> OpenSession();

        #endregion
    }
}