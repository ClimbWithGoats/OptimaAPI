namespace OptimaAPI.Repositories
{
    public partial class ImportDocumentsRepository
    {
        public class Message
        {
            public string? Result { get; set; }
            public string? Errors { get; set; }
        }
    }
}
