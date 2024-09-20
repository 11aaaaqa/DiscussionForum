using CommentMicroservice.Api.Database;
using CommentMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentMicroservice.Api.Services.Repository
{
    public class CommentRepository : IRepository<Comment>
    {
        private readonly ApplicationDbContext context;

        public CommentRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Comment>> GetAllAsync() => await context.Comments.ToListAsync();

        public async Task<List<Comment>?> GetByDiscussionIdAsync(Guid id)
        {
            var discussion = await context.Comments.FirstOrDefaultAsync(x => x.DiscussionId == id);
            if (discussion is null)
            {
                return null;
            }
            return await context.Comments.Where(x => x.DiscussionId == id).ToListAsync();
        }

        public async Task<List<Comment>> GetByIds(params Guid[] ids)
        {
            var comments = new List<Comment>();
            foreach (var id in ids)
            {
                var comment = await context.Comments.SingleOrDefaultAsync(x => x.Id == id);
                if (comment is not null)
                    comments.Add(comment);
            }
            return comments;
        }

        public async Task<List<Comment>> GetByUserName(string userName) =>
            await context.Comments.Where(x => x.CreatedBy == userName).ToListAsync();

        public async Task<Comment?> GetByIdAsync(Guid id) =>
            await context.Comments.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<Comment> UpdateAsync(Comment model)
        {
            context.Comments.Update(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task<Comment> CreateAsync(Comment model)
        {
            await context.Comments.AddAsync(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            var comment = await context.Comments.SingleAsync(x => x.Id == id);
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
        }
    }
}
