using Microsoft.EntityFrameworkCore;

namespace DALTW.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Document> Documents { get; set; }
        
        
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<DocumentFile> DocumentFiles { get; set; }


    }
}
