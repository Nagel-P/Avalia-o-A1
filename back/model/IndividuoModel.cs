using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class Individuo
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Codigo { get; set; }

    [Required]
    public string Nome { get; set; }
}
