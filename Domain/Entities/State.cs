using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class State
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<TaskJob> TaskJobs { get; } = new List<TaskJob>();
}
