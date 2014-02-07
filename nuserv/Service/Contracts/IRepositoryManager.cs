namespace nuserv.Service.Contracts
{
    using System.Collections.Generic;

    using nuserv.Models.Contracts;

    public interface IRepositoryManager
    {
        IEnumerable<IRepository> GetAll();

        IRepository GetById(string id);

        void Add(IRepository repository);

        bool Exists(string id);
    }
}