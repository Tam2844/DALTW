
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace DALTW.Models
{
    public class Document
    {
        public int DocumentID { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        public string? Content { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string? FileURL { get; set; }
        public List<DocumentFile>? DocumentFiles { get; set; }

        public int CategoryID { get; set; }
        public Category? Category { get; set; }

        public int? TopicID { get; set; }
        public Topic? Topic { get; set; }

        public int? GradeID { get; set; }
        public Grade? Grade { get; set; }

        public int? SemesterID { get; set; }
        public Semester? Semester { get; set; }

        public int? CompetitionID { get; set; }
        public Competition? Competition { get; set; }

        public string ImageFilePath { get; set; } = "default_image_path.jpg";

        public int ViewCount { get; set; }
    }
}
