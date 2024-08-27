namespace Web.MVC.Models.ApiResponses
{
    public class TopicResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public uint PostsCount { get; set; }
    }
}
