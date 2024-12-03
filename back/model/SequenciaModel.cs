using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class Sequencia
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string SeqGenetica { get; set; }

    public int? IndividuoId { get; set; }
    
    public Individuo? Individuo { get; set; }
}
