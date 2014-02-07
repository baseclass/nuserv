namespace nuserv.Models
{
    using nuserv.Models.Contracts;

    public abstract class Entity<TId> : IEntity<TId>
    {
        #region Constructors and Destructors

        protected Entity(TId id)
        {
            this.Id = id;
        }

        #endregion

        #region Public Properties

        public TId Id { get; private set; }

        #endregion
    }
}