using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDigital.Models
{
    public class TransactionArticle
    {
        [Key]
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public long ArticleId { get; set; }
        public int ArticleQty { get; set; }

        [ForeignKey("ArticleId")]
        public Article Article { get; set; }

        [ForeignKey("TransactionId")]
        public Transaction Transaction { get; set; }
    }
}
