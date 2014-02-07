namespace nuserv.Models.Contracts
{
    public interface IRepository : IEntity<string>
    {
        #region Public Properties

        string Description { get; }

        string Name { get; }

        #endregion
    }
}