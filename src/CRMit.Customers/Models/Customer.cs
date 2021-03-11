using System;
using System.ComponentModel.DataAnnotations;

namespace CRMit.Customers.Models
{
    /// <summary>
    /// Represents a customer which this CRM system targets.
    /// </summary>
    public class Customer
    {
        public Customer() { }

        public Customer(CustomerInput dto)
        {
            Name = dto.Name;
            Surname = dto.Surname;
            Email = dto.Email;
        }

        /// <summary>
        /// Customer's unique number.
        /// </summary>
        /// <example>123456</example>
        [Required]
        public long Id { get; set; }

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

        public override bool Equals(object obj)
        {
            return obj is Customer customer &&
                   Id == customer.Id &&
                   Name == customer.Name &&
                   Surname == customer.Surname &&
                   Email == customer.Email;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Surname, Email);
        }
    }
}
