using nuserv.Models.Contracts;

namespace nuserv.Models
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public IRepository Create(string id, string name, string description)
        {
            return new Repository(id, name, description);
        }
    }
}