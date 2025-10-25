namespace ChameleonPhotoredactor.Models.Entities
{
    //no encapsulation for timebeing
    public class ImageEdits
    {
        //setters to private and methods for setters, same as in Image class
        public int imageEditId { get; set; }
        //^^could be possibly deleted, `cause on db it will be autoincrement thingy but in code foreign key is enough to associate
        //image and edits
        public int imageIdFK { get; set; }
        //^^idk how to call it otherwise to differintiate it from Models.Entities.Image.imageId
        public float exposureChange { get; set; }
        public float contrastChange { get; set; }
        public float saturationChange { get; set; }
        public float cropData { get; set; }
        public string lastEditDate { get; set; }
        //^^for now idk how to save crop data, may be changed later
        public int timeSpent { get; set; }
        //^^ time spent editing image, so that it counts edited only after some amount of time
        private ImageEdits () { }

        public ImageEdits (int imageEditId, int imageIdFK, float exposureChange, float contrastChange, float saturationChange, float cropData, string lastEditDate)
        {
            this.imageEditId = imageEditId;
            this.imageIdFK = imageIdFK;
            this.exposureChange = exposureChange;
            this.contrastChange = contrastChange;
            this.saturationChange = saturationChange;
            this.cropData = cropData;
            this.lastEditDate = lastEditDate;
        }
    }
}
