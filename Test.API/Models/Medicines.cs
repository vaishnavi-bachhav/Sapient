using System.ComponentModel.DataAnnotations;

namespace Test.API.Models;

public class Medicines 
{
    public int MedicineId { get; set; }

    [Required(ErrorMessage = "Medicine name is required")]
    public string? FullName { get; set; }
    public string? Notes { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Brand is required")]
    public string? Brand { get; set; }
}
