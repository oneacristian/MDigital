using System.ComponentModel.DataAnnotations;

namespace MDigital.Models
{
    public class Article
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
