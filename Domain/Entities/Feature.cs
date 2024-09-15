using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Feature
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; } = new List<Role>();
}
