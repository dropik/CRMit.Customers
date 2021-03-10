using System.ComponentModel.DataAnnotations;

namespace CRMit.Customers.Models
{
    /// <summary>
    /// Customer input data.
    /// </summary>
    public class CustomerInput
    {
        /// <summary>
        /// Name.
        /// </summary>
        /// <example>Ivan</example>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Surname.
        /// </summary>
        /// <example>Petrov</example>
        public string Surname { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        /// <example>ivan.petrov@example.com</example>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
