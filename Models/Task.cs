using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TodoApi.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? InitialDate { get; set; }

    public DateTime? FinishedDate { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; }
}
