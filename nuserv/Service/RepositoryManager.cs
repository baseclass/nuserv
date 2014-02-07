namespace nuserv.Service
{
    using System.Collections.Generic;

    using nuserv.Models.Contracts;
    using nuserv.Service.Contracts;

    public class RepositoryManager : IRepositoryManager
    {
        private readonly IRepositoryFactory repositoryFactory;

        public RepositoryManager(IRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        public void Init()
        {
        }

        public IEnumerable<IRepository> GetAll()
        {
            yield return this.repositoryFactory.Create("test", "test", "test");
        }

        public IRepository GetById(string id)
        {
            return this.repositoryFactory.Create(id, "test", "test");
        }

        public void Add(IRepository repository)
        {
        }
    }
}