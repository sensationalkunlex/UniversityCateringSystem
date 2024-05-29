using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace UniversityCateringSystem.Models
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
    public enum Role
    {
        Admin = 2,
        Customer = 1,

    }
    public class User : BaseEntity
    {

        public string Email { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
