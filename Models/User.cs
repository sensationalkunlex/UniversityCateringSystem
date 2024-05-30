using System.Collections.Generic;
using System.Data;

namespace UniversityCateringSystem.Models
{
    public enum Role
    {
        Admin = 2,
        Customer = 1,

    }
    public class User : BaseEntity
    {

        public string Email { get; set; }
        public string? Name { get; set; }
        public bool? NewUser { get; set; }

        public Role Role { get; set; }
    }
}
