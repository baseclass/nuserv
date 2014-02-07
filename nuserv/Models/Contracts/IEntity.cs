namespace nuserv.Models.Contracts
{
    public interface IEntity<out TId>
    {
        #region Public Properties

        TId Id { get; }

        #endregion
    }
}