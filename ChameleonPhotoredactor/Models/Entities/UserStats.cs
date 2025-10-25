namespace ChameleonPhotoredactor.Models.Entities
{
    //no encapsulation for timebeing
    public class UserStats
    {
        public int statsUserId { get; set;}
        //^^FK to associate UserStats with User
        public int importCount { get; set;}
        public int editCount { get; set;}
        public int exportCount { get; set;}

        private UserStats() { }

        public UserStats(int statsUserId, int importCount, int editCount, int exportCount)
        {
            this.statsUserId = statsUserId;
            this.importCount = importCount;
            this.editCount = editCount;
            this.exportCount = exportCount;
        }
    }
}
