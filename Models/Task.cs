using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoApi.Models;

public partial class Task
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 100 caracteres")]
    public string Title { get; set; } = null!;

    [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "O status é obrigatório")]
    [RegularExpression("^(nao_iniciada|em_progresso|concluida)$", ErrorMessage = "Status inválido. Use: nao_iniciada, em_progresso ou concluida")]
    public string Status { get; set; } = null!;

    public DateTime? InitialDate { get; set; }

    public DateTime? FinishedDate { get; set; }

    [Required(ErrorMessage = "O ID do utilizador é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID do utilizador inválido")]
    public int UserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; }
}
