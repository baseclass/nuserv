namespace nuserv.WebApi.Models
{
    public abstract class Entity<TId>
    {
        #region Public Properties

        public TId Id { get; set; }

        #endregion
    }
}