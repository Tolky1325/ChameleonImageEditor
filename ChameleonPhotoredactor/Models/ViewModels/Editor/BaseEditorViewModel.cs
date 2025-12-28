using System.ComponentModel.DataAnnotations;

namespace ChameleonPhotoredactor.Models.ViewModels.Editor
{
    public class BaseEditorViewModel
    {
        public int ImageEditId { get; set; }

        [Display(Name = "Exposure")]
        public float Exposure { get; set; }

        [Display(Name = "Contrast")]
        public float Contrast { get; set; }

        [Display(Name = "Saturation")]
        public float Saturation { get; set; }

        public string? CropData { get; set; }
    }
}