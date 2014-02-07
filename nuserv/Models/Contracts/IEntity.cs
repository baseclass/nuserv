namespace nuserv.Models.Contracts
{
    public interface IEntity<out TId>
    {
        TId Id { get; }
    }
}