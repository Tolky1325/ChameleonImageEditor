using System.ComponentModel.DataAnnotations;

namespace ChameleonPhotoredactor.Models.ViewModels.Leaderboard
{
    public class LeaderboardViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "Username")]
        public required string Username { get; set; }

        [Display(Name = "Imports")]
        public int Imports { get; set; }

        [Display(Name = "Edits")]
        public int Edits { get; set; }

        [Display(Name = "Exported")]
        public int Exported { get; set; }

        [Display(Name = "Rank")]
        public int Rank { get; set; }
    }
}