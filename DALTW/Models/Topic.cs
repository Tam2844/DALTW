using System.ComponentModel.DataAnnotations;

namespace DALTW.Models
{
    public class Topic
    {
        public int TopicID { get; set; }

        [Required]
        [StringLength(255)]
        public string TopicName { get; set; }

        public ICollection<Document>? Documents { get; set; } = new List<Document>();
    }
}
