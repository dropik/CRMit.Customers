using System.ComponentModel.DataAnnotations;

namespace CRMit.Customers.Models
{
    public class CustomerDTO
    {
        [Required]
        public string Name { get; set; }
        public string Surname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
