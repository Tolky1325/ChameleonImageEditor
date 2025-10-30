using System;
using System.Collections.Generic;

namespace ChameleonPhotoredactor.Models.Entities
{
    //no encapsulation for timebeing
    public class Image
    {
        public int ImageId { get; set; } 
        public string ImageName { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; } 
        //^^png most likely will be only one but for future proofing
        public string? ImageExifData { get; set; } 
        public DateTime ImageUploadDate { get; set; }

        public int UserId { get; set; }
        //^^FK

        public User User { get; set; } = null!;


        public ICollection<ImageEdit> Edits { get; set; } = new List<ImageEdit>();

        private Image() { }

        public Image(int userId, string imageName, byte[] imageData, string imageType, string? imageExifData = null)
        {
            UserId = userId;
            ImageName = imageName;
            ImageData = imageData;
            ImageType = imageType;
            ImageExifData = imageExifData;
            ImageUploadDate = DateTime.UtcNow;
        }
    }
}
