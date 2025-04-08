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

        public async Task<IEnumerable<Competition>> GetAllAsync()
        {
            return await _context.Competitions
                .Include(c => c.Documents)
                .ToListAsync();
        }

        public async Task<Competition> GetByIdAsync(int id)
        {
            return await _context.Competitions
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.CompetitionID == id);
        }

        public async Task AddAsync(Competition competition)
        {
            _context.Competitions.Add(competition);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Competition competition)
        {
            _context.Competitions.Update(competition);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var competition = await _context.Competitions.FindAsync(id);
            if (competition != null)
            {
                _context.Competitions.Remove(competition);
                await _context.SaveChangesAsync();
            }
        }
    }
}
