namespace nuserv.Models
{
    #region Usings

    using nuserv.Models.Contracts;

    #endregion

    public abstract class Entity<TId> : IEntity<TId>
    {
        #region Constructors and Destructors

        protected Entity()
        {
        }

        protected Entity(TId id)
        {
            this.Id = id;
        }

        #endregion

        #region Public Properties

        public TId Id { get; set; }

        #endregion
    }
}