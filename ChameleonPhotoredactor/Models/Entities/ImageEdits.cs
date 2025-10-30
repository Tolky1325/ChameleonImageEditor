using System;

namespace ChameleonPhotoredactor.Models.Entities
{
    //no encapsulation for timebeing
    public class ImageEdit
    {
        public int ImageEditId { get; set; }
        public float ExposureChange { get; set; }
        public float ContrastChange { get; set; }
        public float SaturationChange { get; set; }
        public string? CropData { get; set; }
        //^^idk how to store it, probably JSON
        public DateTime LastEditDate { get; set; }
        public int TimeSpent { get; set; }

        public int ImageId { get; set; }

        public Image Image { get; set; } = null!;

        
        private ImageEdit() { }

        public ImageEdit(int imageId, float exposureChange, float contrastChange, float saturationChange, int timeSpent, string? cropData = null)
        {
            ImageId = imageId;
            ExposureChange = exposureChange;
            ContrastChange = contrastChange;
            SaturationChange = saturationChange;
            TimeSpent = timeSpent;
            CropData = cropData;
            LastEditDate = DateTime.UtcNow;
        }
    }
}