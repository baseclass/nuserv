namespace nuserv.Models
{
    #region Usings

    using nuserv.Models.Contracts;

    #endregion

    public class Repository : Entity<string>, IRepository
    {
        #region Constructors and Destructors

        public Repository()
        {
        }

        public Repository(string id, string name, string description)
            : base(id)
        {
            this.Name = name;
            this.Description = description;
        }

        #endregion

        #region Public Properties

        public string Description { get; set; }

        public string Name { get; set; }

        #endregion
    }
}