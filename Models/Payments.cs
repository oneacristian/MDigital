using MDigital.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MDigital.Models
{
    public class Payment
    {
        [Key]
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        [DefaultValue(Helpers.PaymentStatus.Pending)]
        public int Status { get; set; }
    }
}
