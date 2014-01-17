namespace nuserv.WebApi.Models
{
    public class Repository : Entity<string>
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}