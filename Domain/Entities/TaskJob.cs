using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TaskJob
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string StateId { get; set; } = null!;

    public virtual State State { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
