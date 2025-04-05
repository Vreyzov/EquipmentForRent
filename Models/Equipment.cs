using System;
using System.Collections.Generic;

namespace EquipmentForRent.Models;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string Name { get; set; } = null!;

    public decimal PricePerDay { get; set; }

    public int LessorId { get; set; }

    public virtual Lessor Lessor { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
