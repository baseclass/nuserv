namespace nuserv.Service.Contracts
{
    using System.Collections.Generic;

    using nuserv.Models.Contracts;

    public interface IRepositoryManager
    {
        void Init();

        IEnumerable<IRepository> GetAll();

        IRepository GetById(string id);

        void Add(IRepository repository);
    }
}