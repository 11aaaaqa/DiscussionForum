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

        public async Task<List<Comment>> GetByDiscussionIdAsync(Guid id) =>
            await context.Comments.Where(x => x.DiscussionId == id).ToListAsync();

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
            context.Comments.Remove(new Comment { Id = id });
            await context.SaveChangesAsync();
        }
    }
}
