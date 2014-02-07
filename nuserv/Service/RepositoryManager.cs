namespace nuserv.Service
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using nuserv.Models;
    using nuserv.Models.Contracts;
    using nuserv.Service.Contracts;

    #endregion

    public class RepositoryManager : IRepositoryManager
    {
        #region Fields

        private readonly IRepositoryRepository repository;

        #endregion

        #region Constructors and Destructors

        public RepositoryManager(IRepositoryRepository repository)
        {
            this.repository = repository;
        }

        #endregion

        #region Public Methods and Operators

        public void Add(IRepository repository)
        {
            var repo = repository as Repository;

            if (repo == null)
            {
                throw new ArgumentException("Invalid repository", "repository");
            }

            using (var session = this.repository.OpenSession())
            {
                session.Add(repo);

                session.Commit();
            }
        }

        public bool Exists(string id)
        {
            return this.repository.AsQueryable().Any(r => r.Id == id);
        }

        public IEnumerable<IRepository> GetAll()
        {
            return this.repository.AsQueryable();
        }

        public IRepository GetById(string id)
        {
            return this.repository.AsQueryable().Where(r => r.Id == id).Cast<IRepository>().Single();
        }

        #endregion
    }
}