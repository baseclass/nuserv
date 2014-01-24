namespace nuserv.WebApi.Controllers
{
    #region Usings

    using nuserv.WebApi.Models;
    using System.Collections.Generic;
    using System.Web.Http;

    #endregion

    public class RepositoryController : ApiController
    {
        // GET api/<controller>

        #region Public Methods and Operators

        public void Delete(int id)
        {
        }

        private const string Lorem = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

        public IEnumerable<Repository> Get()
        {
            yield return new Repository() { Id = "ext-curated1", Name = @"External\Curated Feed 1", Description = Lorem };
            yield return new Repository() { Id = "ext-curated2", Name = @"External\Curated Feed 2", Description = Lorem };
            yield return new Repository() { Id = "development", Name = @"Development", Description = Lorem };
            yield return new Repository() { Id = "integration", Name = @"Integration", Description = Lorem };
            yield return new Repository() { Id = "production", Name = @"Production", Description = Lorem };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] Repository repository)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        #endregion

        // DELETE api/<controller>/5
    }
}