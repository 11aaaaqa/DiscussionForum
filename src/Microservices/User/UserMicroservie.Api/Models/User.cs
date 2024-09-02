using Microsoft.EntityFrameworkCore;

namespace UserMicroservice.Api.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User
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
    }
}
