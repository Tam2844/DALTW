using DALTW.Models;
using Microsoft.EntityFrameworkCore;

namespace DALTW.Repositories
{
    public class EFGradeRepository: IGradeRepository
    {
        private readonly ApplicationDbContext _context;
        public EFGradeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Grade grade)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Grade>> GetAllAsync()
        {
            return await _context.Grades
            .Include(p => p.Documents)
            .ToListAsync();
        }
        public async Task<Grade> GetByIdAsync(int id)
        {
            return await _context.Grades.Include(p =>
            p.Documents).FirstOrDefaultAsync(p => p.GradeID == id);
        }

        public Task UpdateAsync(Grade grade)
        {
            throw new NotImplementedException();
        }
    }
}
