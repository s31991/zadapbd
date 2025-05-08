
using System.ComponentModel.DataAnnotations;
namespace WebApplication1.DTOs;

public class ClientDTO
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(120, ErrorMessage = "First name cannot exceed 120 characters")]
    public string FirstName { get; set; }
        
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(120, ErrorMessage = "Last name cannot exceed 120 characters")]
    public string LastName { get; set; }
        
    [Required(ErrorMessage = "Email is required")]
    [StringLength(120, ErrorMessage = "Email cannot exceed 120 characters")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
        
    [StringLength(120, ErrorMessage = "Telephone cannot exceed 120 characters")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string Telephone { get; set; }
        
    [StringLength(120, ErrorMessage = "PESEL cannot exceed 120 characters")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL must be exactly 11 digits")]
    public string Pesel { get; set; }
}