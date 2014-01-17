namespace nuserv.WebApi.Controllers
{
    #region Usings

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

        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
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