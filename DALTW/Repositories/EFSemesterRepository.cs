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

        public Task AddAsync(Semester semester)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Semester>> GetAllAsync()
        {
            return await _context.Semesters
            .Include(p => p.Documents)
            .ToListAsync();
        }

        public async Task<Semester> GetByIdAsync(int id)
        {
            return await _context.Semesters.Include(p =>
           p.Documents).FirstOrDefaultAsync(p => p.SemesterID == id);
        }

        public Task UpdateAsync(Semester semester)
        {
            throw new NotImplementedException();
        }
    }
}
