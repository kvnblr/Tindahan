using System.ComponentModel.DataAnnotations;

namespace Api.Entities.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(20, ErrorMessage = "Maximum length of NAME must be 20 characters")]
    public string? Name { get; set; }
    [Required]
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
