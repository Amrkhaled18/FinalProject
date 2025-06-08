using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectTest.Models
{
    public class Preference
    {
        public int PreferenceID { get; set; }
        public string DietaryRestrictions { get; set; }
        public string FavoriteCuisines { get; set; }
        public string AccessibilityRequirements { get; set; }

        // Add User linkage
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
