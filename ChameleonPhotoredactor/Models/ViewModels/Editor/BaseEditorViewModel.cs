namespace ChameleonPhotoredactor.Models.ViewModels.Editor
{
    public class BaseEditorViewModel
    {
        public int ImageEditId { get; set; }
        public float Exposure { get; set; }
        public float Contrast { get; set; }
        public float Saturation { get; set; }
        public string? CropData { get; set; }
    }
}
