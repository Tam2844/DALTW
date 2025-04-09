using DALTW.Models;
namespace DALTW.Repositories
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetAllAsync();
        Task<Document> GetByIdAsync(int id);
        Task AddAsync(Document document);
        Task UpdateAsync(Document document);
        Task DeleteAsync(int id);
        Task<List<Document>> GetNewestDocumentsAsync(int count);

    }
}
