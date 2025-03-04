using System;
using System.Collections.Generic;

namespace csharp_choreography_saga.OrderMicroservice.Entities;

public partial class TblUser
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();
}
