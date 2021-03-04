using System;

namespace CRMit.Customers.Models
{
    public class Customer
    {
        public Customer() { }

        public Customer(CustomerDTO dto)
        {
            Name = dto.Name;
            Surname = dto.Surname;
            Email = dto.Email;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
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
