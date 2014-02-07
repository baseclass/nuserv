namespace nuserv.WebApi.Models
{
    public class RepositoryViewModel : EntityViewModel<string>
    {
        #region Public Properties

        public string Description { get; set; }

        public string Name { get; set; }

        #endregion
    }
}