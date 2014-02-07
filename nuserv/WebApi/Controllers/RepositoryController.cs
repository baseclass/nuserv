namespace nuserv.WebApi.Controllers
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using nuserv.Models.Contracts;
    using nuserv.Service.Contracts;
    using nuserv.WebApi.Models;

    #endregion

    public class RepositoryController : ApiController
    {
        #region Fields

        private readonly IRepositoryFactory repositoryFactory;

        private readonly IRepositoryManager repositoryManager;

        #endregion

        #region Constructors and Destructors

        public RepositoryController(IRepositoryManager repositoryManager, IRepositoryFactory repositoryFactory)
        {
            this.repositoryManager = repositoryManager;
            this.repositoryFactory = repositoryFactory;
        }

        #endregion

        #region Public Methods and Operators

        /// <remarks>
        ///     DELETE api/controller/5
        /// </remarks>
        public void Delete(int id)
        {
        }

        /// <remarks>
        ///     GET api/controller
        /// </remarks>
        public IEnumerable<RepositoryViewModel> Get()
        {
            return
                this.repositoryManager.GetAll()
                    .Select(
                        repository =>
                            new RepositoryViewModel()
                            {
                                Id = repository.Id,
                                Name = repository.Name,
                                Description = repository.Description
                            });
        }

        /// <remarks>
        ///     GET api/controller/5
        /// </remarks>
        public RepositoryViewModel Get(string id)
        {
            var repository = this.repositoryManager.GetById(id);

            var repositoryViewModel = new RepositoryViewModel()
                                      {
                                          Id = repository.Id,
                                          Name = repository.Name,
                                          Description = repository.Description
                                      };

            return repositoryViewModel;
        }

        /// <remarks>
        ///     POST api/controller
        /// </remarks>
        public void Post([FromBody] RepositoryViewModel repositoryViewModel)
        {
            var repository = this.repositoryFactory.Create(
                repositoryViewModel.Id,
                repositoryViewModel.Name,
                repositoryViewModel.Description);

            this.repositoryManager.Add(repository);
        }

        /// <remarks>
        ///     PUT api/controller/5
        /// </remarks>
        public void Put(int id, [FromBody] string value)
        {
        }

        #endregion
    }
}