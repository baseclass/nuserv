namespace nuserv.WebApi.Models
{
    public abstract class EntityViewModel<TId>
    {
        #region Public Properties

        public TId Id { get; set; }

        #endregion
    }
}