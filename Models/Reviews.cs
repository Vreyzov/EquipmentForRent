using System;

namespace EquipmentForRent.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }  // Идентификатор отзыва
        public int OrderId { get; set; }   // Идентификатор заказа
        public int ClientId { get; set; }  // Идентификатор клиента
        public int LessorId { get; set; }  // Идентификатор арендодателя
        public int Rating { get; set; }    // Оценка (1-5)
        public string? ReviewText { get; set; }  // Текст отзыва (необязательный)
        public DateTime CreatedAt { get; set; }  // Дата создания

     

        // Навигационные свойства
        public virtual Client Client { get; set; }
        public virtual Lessor Lessor { get; set; }
        public virtual Order Order { get; set; }
    }
}
