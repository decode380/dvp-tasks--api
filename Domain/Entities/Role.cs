using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Role
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Feature> Features { get; } = new List<Feature>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}
