using System;
using System.Collections.Generic;

namespace csharp_choreography_saga.StockMicroservice.Entities;

public partial class TblOrder
{
    public Guid OrderId { get; set; }

    public Guid UserId { get; set; }

    public Guid InvoiceNo { get; set; }

    public double GrandTotal { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TblOrderDetail> TblOrderDetails { get; set; } = new List<TblOrderDetail>();

    public virtual TblUser User { get; set; } = null!;
}
