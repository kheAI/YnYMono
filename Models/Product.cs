using System.ComponentModel.DataAnnotations;

namespace YnYMono.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}