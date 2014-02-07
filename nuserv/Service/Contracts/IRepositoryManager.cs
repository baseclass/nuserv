namespace nuserv.Service.Contracts
{
    #region Usings

    using System.Collections.Generic;

    using nuserv.Models.Contracts;

    #endregion

    public interface IRepositoryManager
    {
        #region Public Methods and Operators

        void Add(IRepository repository);

        bool Exists(string id);

        IEnumerable<IRepository> GetAll();

        IRepository GetById(string id);

        #endregion
    }
}