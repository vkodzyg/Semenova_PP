using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace semenova_library
{
    public class Product
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("article")]
        public string Article { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("unit")]
        public string Unit { get; set; } = string.Empty;

        [Column("price")]
        public decimal Price { get; set; }

        public virtual ICollection<SalesHistory> SalesHistories { get; set; } = new List<SalesHistory>();
    }
}