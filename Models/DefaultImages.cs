namespace EquipmentForRent.Models
{
    public partial class DefaultImage
    {
        public int ImageID { get; set; } // Идентификатор изображения
        public string ImageName { get; set; } // Имя изображения
        public byte[] ImageData { get; set; } // Данные изображения
    }
}
