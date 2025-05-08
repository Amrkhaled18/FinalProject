using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FinalProjectTest.Models
{
    public class LocationImage
    {
        [Key]
        public int ImageID { get; set; }

        [Required]
        public string ImageURL { get; set; }

        [ForeignKey("Location")]
        public int LocationID { get; set; }

        public Location Location { get; set; }
    }
}
