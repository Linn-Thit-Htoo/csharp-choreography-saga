using System;
using System.Collections.Generic;

namespace csharp_choreography_saga.StockMicroservice.Entities;

public partial class OneTimeToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public string RelatesTo { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
