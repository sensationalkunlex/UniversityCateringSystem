using System.ComponentModel.DataAnnotations;

namespace UniversityCateringSystem.Models
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
