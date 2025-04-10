using DALTW.Models;
using Microsoft.EntityFrameworkCore;

namespace DALTW.Repositories
{
    public class EFDocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;
        public EFDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents
            .Include(p => p.Category)
            .Include(d => d.Grade)
            .Include(d => d.Topic)
            .ToListAsync();
        }
        public async Task<Document> GetByIdAsync(int id)
        {
            return await _context.Documents.Include(p =>
           p.Category).FirstOrDefaultAsync(p => p.DocumentID == id);
        }
        public async Task AddAsync(Document document)
        {
            try
            {
                _context.Documents.Add(document);
                Console.WriteLine($"Document: {document.Name}, Category: {document.CategoryID}, Topic: {document.TopicID}, Grade: {document.GradeID}");
                await _context.SaveChangesAsync();
                Console.WriteLine("Document saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving document: {ex.Message}");
            }
        }

        public async Task UpdateAsync(Document document)
        {
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Document>> GetNewestDocumentsAsync(int count)
        {
            return await _context.Documents
                .Include(d => d.Category)
                .Include(d => d.Topic)
                .Include(d => d.Grade)
                .OrderByDescending(d => d.CreatedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Document> GetBySlugAsync(string slug)
        {
            return await _context.Documents.FirstOrDefaultAsync(p => p.Slug == slug);
        }

    }
}
