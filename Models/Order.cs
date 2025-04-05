using System;
using System.Collections.Generic;

namespace EquipmentForRent.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime? OrderDate { get; set; }

    public int ClientId { get; set; }

    public int LessorId { get; set; }

    public int EquipmentId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal TotalCost { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual Lessor Lessor { get; set; } = null!;

    public string LessorFullName => Lessor != null ? $"{Lessor.FirstName} {Lessor.LastName}" : "Неизвестен";


}
