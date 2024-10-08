﻿using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TaskJob> TaskJobs { get; } = new List<TaskJob>();
}
