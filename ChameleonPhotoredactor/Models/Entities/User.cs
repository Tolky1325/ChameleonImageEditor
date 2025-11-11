namespace ChameleonPhotoredactor.Models.Entities
{
    //no encapsulation for timebeing
    public class User
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string userDisplayName { get; set; }
        public string userEmail { get; set; }
        public string userPassword { get; set; }
        //^^later on think about hashing
        public byte[]? userProfilePic { get; set; }
        public DateTime userCreationDate { get; set; }
        public bool isTemp { get; set; }

        public UserStats? UserStats { get; set; }
        public ICollection<Image> Images { get; set; } = new List<Image>();
        public string Username { get; internal set; }
        private User() { }

        public User ( string userName, string userDisplayName, string userEmail, string userPassword, byte[] userProfilePic, DateTime userCreationDate, bool isTemp)
        {
            this.userName = userName;
            this.userDisplayName = userDisplayName;
            this.userEmail = userEmail;
            this.userPassword = userPassword;
            this.userProfilePic = userProfilePic;
            this.userCreationDate = userCreationDate;
            this.isTemp = isTemp;
        }
    }
}
