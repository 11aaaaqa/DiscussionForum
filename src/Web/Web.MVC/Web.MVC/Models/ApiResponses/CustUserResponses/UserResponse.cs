namespace Web.MVC.Models.ApiResponses.CustUserResponses
{
    public class UserResponse
    {
        public string AspNetUserId { get; set; }
        public Guid Id { get; set; }
        public List<Guid> CreatedDiscussionsIds { get; set; }
        public List<Guid> SuggestedDiscussionsIds { get; set; }
        public List<Guid> SuggestedCommentsIds { get; set; }
        public List<Guid> CommentsIds { get; set; }
        public string UserName { get; set; }
        public uint Posts { get; set; }
        public uint Answers { get; set; }
        public DateOnly RegisteredAt { get; set; }
        public bool IsBanned { get; set; }
        public DateTime BannedUntil { get; set; }
        public string BannedFor { get; set; }
        public string BanType { get; set; }
    }
}
