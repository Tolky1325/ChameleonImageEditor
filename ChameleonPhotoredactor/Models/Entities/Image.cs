namespace ChameleonPhotoredactor.Models.Entities
{
    //no encapsulation for timebeing
    public class Image
    {
        //later on change setters to private i suppose
        //also add methods for setters
        public int imageId {  get; set; }
        public int imageUserId { get; set; }
        public string imageName { get; set; }
        public byte[] imageData { get; set; }
        public string imageType { get; set; }
        public string imageExifData { get; set; }
        public string imageUploadDate { get; set; }

        private Image() { }

        public Image(int imageId, int imageUserId, string imageName, byte[] imageData, string imageType, string imageExifData, string imageUploadDate)
        {
            this.imageId = imageId;
            this.imageUserId = imageUserId;
            this.imageName = imageName;
            this.imageData = imageData;
            this.imageType = imageType;
            this.imageExifData = imageExifData;
            this.imageUploadDate = imageUploadDate;
        }
    }

    
}
