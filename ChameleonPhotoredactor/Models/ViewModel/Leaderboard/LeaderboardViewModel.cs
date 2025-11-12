namespace ChameleonPhotoredactor.Models.ViewModels.Leaderboard
{
    public class LeaderboardViewModel
    {
        public int UserId { get; set; }

        public required string Username { get; set; }

        public int Imports { get; set; }
        public int Edits { get; set; }
        public int Exported { get; set; }

        public int Rank { get; set; }
    }
}
