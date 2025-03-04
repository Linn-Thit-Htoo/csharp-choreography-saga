using System;
using System.Collections.Generic;

namespace csharp_choreography_saga.StockMicroservice.Entities;

public partial class SchemaMigration
{
    public long Version { get; set; }

    public DateTime? InsertedAt { get; set; }
}
