namespace nuserv.Models.Contracts
{
    public interface IRepository : IEntity<string>
    {
        string Name { get; }

        string Description { get; }
    }
}