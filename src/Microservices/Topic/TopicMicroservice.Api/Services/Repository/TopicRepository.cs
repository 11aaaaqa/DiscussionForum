using Microsoft.EntityFrameworkCore;
using TopicMicroservice.Api.Database;
using TopicMicroservice.Api.Models;

namespace TopicMicroservice.Api.Services.Repository
{
    public class TopicRepository : IRepository<Topic>
    {
        private readonly ApplicationDbContext context;

        public TopicRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Topic>> GetAllAsync(int pageSize, int pageNumber, string searchingString)
        {
            var topics = await context.Topics.Where(x => x.Name.Contains(searchingString))
                .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return topics;
        }

        public async Task<List<Topic>> GetAllAsync(int pageSize, int pageNumber)
        {
            var topics = await context.Topics.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return topics;
        }

        public async Task<Topic?> GetByIdAsync(Guid id) => await context.Topics.SingleOrDefaultAsync(x => x.Id == id);


        public async Task<Topic?> GetByNameAsync(string name) => await context.Topics.SingleOrDefaultAsync(x => x.Name == name);

        public async Task<Topic> UpdateAsync(Topic model)
        {
            context.Topics.Update(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task<Topic> CreateAsync(Topic model)
        {
           await context.Topics.AddAsync(model);
           await context.SaveChangesAsync();
           return model;
        }

        public async Task DeleteByNameAsync(string name)
        {
            context.Topics.Remove(new Topic { Name = name });
            await context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            context.Topics.Remove(new Topic { Id = id });
            await context.SaveChangesAsync();
        }
    }
}
