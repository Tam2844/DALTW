using DALTW.Models;
using Microsoft.EntityFrameworkCore;

namespace DALTW.Repositories
{
    public class EFTopicRepository : ITopicRepository
    {
        private readonly ApplicationDbContext _context;

        public EFTopicRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Topic topic)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Topic>> GetAllAsync()
        {
            return await _context.Topics
            .Include(p => p.Documents) 
            .ToListAsync();
        }

        public async Task<Topic> GetByIdAsync(int id)
        {
            return await _context.Topics.Include(p =>
            p.Documents).FirstOrDefaultAsync(p => p.TopicID == id);
        }

        public Task UpdateAsync(Topic topic)
        {
            throw new NotImplementedException();
        }
    }
}
