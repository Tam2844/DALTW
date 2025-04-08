using DALTW.Models;
using Microsoft.EntityFrameworkCore;
using DALTW.Repositories;

namespace DALTW.Repositories
{
    public class EFGradeRepository : IGradeRepository
    {
        private readonly ApplicationDbContext _context;

        public EFGradeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Grade>> GetAllAsync()
        {
            return await _context.Grades
                .Include(g => g.Documents)
                .ToListAsync();
        }

        public async Task<Grade> GetByIdAsync(int id)
        {
            return await _context.Grades
                .Include(g => g.Documents)
                .FirstOrDefaultAsync(g => g.GradeID == id);
        }

        public async Task AddAsync(Grade grade)
        {
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Grade grade)
        {
            _context.Grades.Update(grade);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
                await _context.SaveChangesAsync();
            }
        }
    }
}
