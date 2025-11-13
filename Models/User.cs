using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly? BirthdayDate { get; set; }

    public string? Working { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
