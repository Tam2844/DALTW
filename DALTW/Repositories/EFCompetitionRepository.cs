using DALTW.Models;
using Microsoft.EntityFrameworkCore;

namespace DALTW.Repositories
{
    public class EFCompetitionRepository : ICompetitionRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCompetitionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Competition competition)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Competition>> GetAllAsync()
        {
            return await _context.Competitions
            .Include(p => p.Documents)
            .ToListAsync();
        }

        public async Task<Competition> GetByIdAsync(int id)
        {
            return await _context.Competitions.Include(p =>
           p.Documents).FirstOrDefaultAsync(p => p.CompetitionID == id);
        }

        public Task UpdateAsync(Competition competition)
        {
            throw new NotImplementedException();
        }
    }
}
