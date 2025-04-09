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

        public async Task<IEnumerable<Topic>> GetAllAsync()
        {
            return await _context.Topics
                .Include(t => t.Documents)
                .ToListAsync();
        }

        public async Task<Topic> GetByIdAsync(int id)
        {
            return await _context.Topics
                .Include(t => t.Documents)
                .FirstOrDefaultAsync(t => t.TopicID == id);
        }

        public async Task AddAsync(Topic topic)
        {
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Topic topic)
        {
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic != null)
            {
                _context.Topics.Remove(topic);
                await _context.SaveChangesAsync();
            }
        }


    }
}
