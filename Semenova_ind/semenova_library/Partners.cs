using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace semenova_library
{
    public class Partners
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("type_id")]
        public int TypeId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("legal_adress")]
        public string LegalAdress { get; set; }

        [Column("inn")]
        public string Inn { get; set; }

        [Column("director_fullname")]
        public string DirectorFullname { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("logo_path")]
        public string LogoPath { get; set; }

        [Column("rating")]
        public int Rating { get; set; }

        public virtual Partner_Types Type { get; set; }
        public virtual ICollection<SalesHistory> SalesHistories { get; set; } = new List<SalesHistory>();

        public decimal GetTotalSalesAmount(IEnumerable<SalesHistory> sales)
        {
            decimal total = 0;
            foreach (var sale in sales)
            {
                if (sale.Product != null)
                {
                    total += sale.Quantity * sale.Product.Price;
                }
            }
            return total;
        }

        public int CalculateDiscount(decimal totalSalesAmount)
        {
            if (totalSalesAmount < 10000)
                return 0;
            else if (totalSalesAmount < 50000)
                return 5;
            else if (totalSalesAmount < 300000)
                return 10;
            else
                return 15;
        }

        public decimal GetTotalSalesAmountFromDb()
        {
            if (SalesHistories == null || !SalesHistories.Any()) return 0;
            decimal total = 0;
            foreach (var sale in SalesHistories)
            {
                if (sale.Product != null)
                {
                    total += sale.Quantity * sale.Product.Price;
                }
            }
            return total;
        }

        public int GetCurrentDiscount()
        {
            decimal totalSales = GetTotalSalesAmountFromDb();
            return CalculateDiscount(totalSales);
        }

        [NotMapped]
        public int CurrentDiscount => GetCurrentDiscount();
    }
}