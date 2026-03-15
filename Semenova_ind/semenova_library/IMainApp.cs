using System;
using System.Collections.Generic;

namespace semenova_library
{
    public interface IMainApp
    {
        event Action PartnersChanged;
        event Action DataLoaded;
        event Action<string> ErrorOccurred;

        void AddPartner(Partners partner);
        void UpdatePartner(Partners partner);
        void RemovePartner(int id);
        IEnumerable<Partners> GetAllPartners();
        Partners GetPartnerById(int id);

        IEnumerable<Partner_Types> GetAllPartnerTypes();

        IEnumerable<SalesHistory> GetPartnerSalesHistory(int partnerId);
        void AddSalesRecord(SalesHistory salesRecord);

        IEnumerable<Partners> SearchPartners(string searchTerm);
        IEnumerable<Partners> GetPartnersByType(int typeId);
        IEnumerable<Partners> GetPartnersByRating(int minRating, int maxRating);

        decimal GetPartnerTotalSales(int partnerId);
        int CalculatePartnerDiscount(decimal totalSalesAmount);
        int GetPartnerCurrentDiscount(int partnerId);

        int GetTotalPartnersCount();
        int GetPartnersWithHighRatingCount(int minRating);
        Dictionary<string, int> GetPartnersDistributionByType();

        void LoadData(); 
        void SaveData(); 

        Partners SelectedPartner { get; set; }
        string StatusMessage { get; set; }
        string SearchTerm { get; set; }
        int? SelectedTypeId { get; set; }

        IEnumerable<Partners> GetFilteredPartners();

        IEnumerable<Partners> GetPartnersSortedByName();
        IEnumerable<Partners> GetPartnersSortedByRating();
        IEnumerable<Partners> GetPartnersSortedBySales();
      
        bool IsInnUnique(string inn, int? excludePartnerId = null);
        (bool IsValid, string ErrorMessage) ValidatePartner(Partners partner);

        IEnumerable<Product> GetAllProducts();

        Product FindProductByName(string name);

        void AddProduct(Product product);

        
    }
}
