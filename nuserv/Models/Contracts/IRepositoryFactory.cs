namespace nuserv.Models.Contracts
{
    public interface IRepositoryFactory
    {
        #region Public Methods and Operators

        IRepository Create(string id, string name, string description);

        #endregion
    }
}