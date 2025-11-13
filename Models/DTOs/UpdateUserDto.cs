using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models.DTOs;

public class UpdateUserDto
{
    [Required(ErrorMessage = "O ID é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID inválido")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email não pode exceder 100 caracteres")]
    public string Email { get; set; } = null!;

    public DateOnly? BirthdayDate { get; set; }

    [StringLength(100, ErrorMessage = "O cargo/função não pode exceder 100 caracteres")]
    public string? Working { get; set; }
}
