using System.ComponentModel.DataAnnotations;
namespace DALTW.Models
{
    public class DocumentFile
    {
        [Key]
        public int FileID { get; set; }
        
        public string FileURL { get; set; }

        public int DocumentID { get; set; }
        public Document? Document { get; set; }
    }
}
