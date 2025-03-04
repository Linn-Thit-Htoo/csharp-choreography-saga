using System;
using System.Collections.Generic;

namespace csharp_choreography_saga.OrderMicroservice.Entities;

public partial class TblStock
{
    public Guid StockId { get; set; }

    public Guid ProductId { get; set; }

    public long Stock { get; set; }

    public virtual TblProduct Product { get; set; } = null!;
}
