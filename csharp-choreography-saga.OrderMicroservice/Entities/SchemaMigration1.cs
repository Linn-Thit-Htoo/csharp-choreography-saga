using System;
using System.Collections.Generic;

namespace csharp_choreography_saga.OrderMicroservice.Entities;

/// <summary>
/// Auth: Manages updates to the auth system.
/// </summary>
public partial class SchemaMigration1
{
    public string Version { get; set; } = null!;
}
