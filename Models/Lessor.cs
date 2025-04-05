using System;
using System.Collections.Generic;

namespace EquipmentForRent.Models
{
    public partial class Lessor
    {
        public int LessorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public byte[]? Avatar { get; set; } // Теперь поле может быть NULL
        public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } // Связь с отзывами
    }
}
