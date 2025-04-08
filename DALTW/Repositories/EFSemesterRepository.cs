using DALTW.Models;
using Microsoft.EntityFrameworkCore;

namespace DALTW.Repositories
{
    public class EFSemesterRepository : ISemesterRepository
    {
        private readonly ApplicationDbContext _context;

        public EFSemesterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Semester>> GetAllAsync()
        {
            return await _context.Semesters
                .Include(s => s.Documents)
                .ToListAsync();
        }

        public async Task<Semester> GetByIdAsync(int id)
        {
            return await _context.Semesters
                .Include(s => s.Documents)
                .FirstOrDefaultAsync(s => s.SemesterID == id);
        }

        public async Task AddAsync(Semester semester)
        {
            _context.Semesters.Add(semester);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);
            if (semester != null)
            {
                _context.Semesters.Remove(semester);
                await _context.SaveChangesAsync();
            }
        }
    }
}
