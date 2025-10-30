using System.ComponentModel.DataAnnotations;

namespace ChameleonPhotoredactor.Models.Entities
{
    //no encapsulation for timebeing
    public class UserStats
    {
        [Key]
        public int UserId { get; set; }
        //^^FK to associate UserStats with User
        public int importCount { get; set;}
        public int editCount { get; set;}
        public int exportCount { get; set;}

        public User User { get; set; } = null!;
        private UserStats() { }

        public UserStats(int userId, int importCount, int editCount, int exportCount)
        {
            this.UserId = userId;
            this.importCount = importCount;
            this.editCount = editCount;
            this.exportCount = exportCount;
        }
    }
}
