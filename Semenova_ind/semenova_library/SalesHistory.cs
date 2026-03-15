using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace semenova_library
{
    public class SalesHistory
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("partner_id")]
        public int PartnerId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("sale_date")]
        public DateTime SaleDate { get; set; }

        public virtual Partners Partner { get; set; } = null;
        public virtual Product Product { get; set; } = null;
    }
}