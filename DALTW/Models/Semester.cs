using System.ComponentModel.DataAnnotations;

namespace DALTW.Models
{
    public class Semester
    {
        public int SemesterID { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public ICollection<Document>? Documents { get; set; }
    }
}
