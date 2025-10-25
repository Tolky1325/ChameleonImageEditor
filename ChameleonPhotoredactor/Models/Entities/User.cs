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
        public byte[] userProfilePic { get; set; }
        public string userCreationDate { get; set; }
        //     ^^^^^^ DateTime type mb
        public bool isTemp { get; set; }

        private User() { }

        public User (int userId, string userName, string userDisplayName, string userEmail, string userPassword, byte[] userProfilePic, string userCreationDate, bool isTemp)
        {
            this.userId = userId;
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
