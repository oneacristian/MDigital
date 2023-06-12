using MDigital.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDigital.Models
{
    public class Transaction
    {
        [Key]
        public long Id { get; set; }
        public long PaymentId { get; set; }
        public long CustomerId { get; set; }

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
