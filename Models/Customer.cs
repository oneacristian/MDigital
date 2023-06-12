using System.ComponentModel.DataAnnotations;

namespace MDigital.Models
{
    public class Customer
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
