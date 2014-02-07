namespace nuserv.WebApi.Models
{
    public class RepositoryViewModel : EntityViewModel<string>
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}