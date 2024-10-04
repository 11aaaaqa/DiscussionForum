namespace Web.MVC.Models.ApiResponses
{
    public class TopicModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public uint PostsCount { get; set; }
    }
}
