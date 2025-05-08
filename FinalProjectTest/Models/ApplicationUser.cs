using Microsoft.AspNetCore.Identity;

namespace FinalProjectTest.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? PreferencesID { get; set; }
        public Preference? Preferences { get; set; }

        public ICollection<Recommendation>? Recommendations { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
        public ICollection<ChatbotInteraction>? ChatbotInteractions { get; set; }
    }
}
