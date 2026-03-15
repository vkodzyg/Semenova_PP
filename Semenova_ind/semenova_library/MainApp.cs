using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace semenova_library
{
    public class MainApp : IMainApp
    {
        private readonly AppDbContext _context;
        private List<Partners> _partners;
        private List<Partner_Types> _partnerTypes;
        private List<SalesHistory> _salesHistories;

        public event Action PartnersChanged;
        public event Action DataLoaded;
        public event Action<string> ErrorOccurred;

        public Partners SelectedPartner { get; set; }
        public string StatusMessage { get; set; } = "Готово";
        public string SearchTerm { get; set; }
        public int? SelectedTypeId { get; set; }

        public MainApp()
        {
            _context = new AppDbContext();
            _partners = new List<Partners>();
            _partnerTypes = new List<Partner_Types>();
            _salesHistories = new List<SalesHistory>();
        }

        public void LoadData()
        {
            try
            {

                _partnerTypes = _context.PartnerTypes
                    .OrderBy(t => t.Name)
                    .ToList();


                _partners = _context.Partner
                    .Include(p => p.Type)
                    .Include(p => p.SalesHistories)
                        .ThenInclude(s => s.Product)
                    .OrderBy(p => p.Name)
                    .ToList();


                _salesHistories = _context.SalesHistories
                    .Include(s => s.Product)
                    .Include(s => s.Partner)
                    .ToList();

                StatusMessage = $"Загружено партнеров: {_partners.Count}";
                DataLoaded?.Invoke();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки: {ex.Message}";
                ErrorOccurred?.Invoke($"Ошибка подключения к базе данных: {ex.Message}");
            }
        }

        public void SaveData()
        {
            try
            {
                _context.SaveChanges();
                StatusMessage = "Данные сохранены";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка сохранения: {ex.Message}";
                ErrorOccurred?.Invoke($"Ошибка сохранения: {ex.Message}");
            }
        }

        public void AddPartner(Partners partner)
        {
            try
            {

                var validation = ValidatePartner(partner);
                if (!validation.IsValid)
                {
                    ErrorOccurred?.Invoke(validation.ErrorMessage);
                    return;
                }

                if (!string.IsNullOrEmpty(partner.Inn) && !IsInnUnique(partner.Inn))
                {
                    ErrorOccurred?.Invoke("Партнер с таким ИНН уже существует");
                    return;
                }

                _context.Partner.Add(partner);
                _context.SaveChanges();

                _partners.Add(partner);

                StatusMessage = $"Партнер '{partner.Name}' добавлен";
                PartnersChanged?.Invoke();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка добавления: {ex.Message}";
                ErrorOccurred?.Invoke($"Ошибка при добавлении партнера: {ex.Message}");
            }
        }

        public void UpdatePartner(Partners updatedPartner)
        {
            try
            {
                var validation = ValidatePartner(updatedPartner);
                if (!validation.IsValid)
                {
                    ErrorOccurred?.Invoke(validation.ErrorMessage);
                    return;
                }

                if (!string.IsNullOrEmpty(updatedPartner.Inn) &&
                    !IsInnUnique(updatedPartner.Inn, updatedPartner.Id))
                {
                    ErrorOccurred?.Invoke("Партнер с таким ИНН уже существует");
                    return;
                }

                var existingPartner = _context.Partner.Find(updatedPartner.Id);
                if (existingPartner != null)
                {
                    existingPartner.TypeId = updatedPartner.TypeId;
                    existingPartner.Name = updatedPartner.Name;
                    existingPartner.LegalAdress = updatedPartner.LegalAdress;
                    existingPartner.Inn = updatedPartner.Inn;
                    existingPartner.DirectorFullname = updatedPartner.DirectorFullname;
                    existingPartner.Phone = updatedPartner.Phone;
                    existingPartner.Email = updatedPartner.Email;
                    existingPartner.LogoPath = updatedPartner.LogoPath;
                    existingPartner.Rating = updatedPartner.Rating;

                    _context.SaveChanges();

                    var index = _partners.FindIndex(p => p.Id == updatedPartner.Id);
                    if (index >= 0)
                    {
                        _partners[index] = _context.Partner
                            .Include(p => p.Type)
                            .Include(p => p.SalesHistories)
                            .First(p => p.Id == updatedPartner.Id);
                    }

                    StatusMessage = $"Партнер '{updatedPartner.Name}' обновлен";
                    PartnersChanged?.Invoke();
                }
                else
                {
                    ErrorOccurred?.Invoke($"Партнер с ID {updatedPartner.Id} не найден!");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка обновления: {ex.Message}";
                ErrorOccurred?.Invoke($"Ошибка при обновлении партнера: {ex.Message}");
            }
        }

        public void RemovePartner(int id)
        {
            try
            {
                var partner = _context.Partner.Find(id);
                if (partner != null)
                {
                    _context.Partner.Remove(partner);
                    _context.SaveChanges();

                    _partners.RemoveAll(p => p.Id == id);

                    StatusMessage = $"Партнер удален";
                    PartnersChanged?.Invoke();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка удаления: {ex.Message}";
                ErrorOccurred?.Invoke($"Ошибка при удалении партнера: {ex.Message}");
            }
        }

        public IEnumerable<Partners> GetAllPartners()
        {
            return _partners;
        }

        public Partners GetPartnerById(int id)
        {
            return _partners.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Partner_Types> GetAllPartnerTypes()
        {
            return _partnerTypes;
        }

        public IEnumerable<SalesHistory> GetPartnerSalesHistory(int partnerId)
        {
            return _salesHistories
                .Where(s => s.PartnerId == partnerId)
                .OrderByDescending(s => s.SaleDate);
        }

        public void AddSalesRecord(SalesHistory salesRecord)
        {
            try
            {
                _context.SalesHistories.Add(salesRecord);
                _context.SaveChanges();

                _salesHistories.Add(salesRecord);

                StatusMessage = "Запись о продаже добавлена";
                PartnersChanged?.Invoke(); 
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка добавления продажи: {ex.Message}";
                ErrorOccurred?.Invoke($"Ошибка при добавлении продажи: {ex.Message}");
            }
        }

        public IEnumerable<Partners> SearchPartners(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _partners;

            searchTerm = searchTerm.ToLower();

            return _partners.Where(p =>
                (p.Name?.ToLower().Contains(searchTerm) ?? false) ||
                (p.DirectorFullname?.ToLower().Contains(searchTerm) ?? false) ||
                (p.Phone?.Contains(searchTerm) ?? false) ||
                (p.Email?.ToLower().Contains(searchTerm) ?? false) ||
                (p.Inn?.Contains(searchTerm) ?? false) ||
                (p.LegalAdress?.ToLower().Contains(searchTerm) ?? false)
            );
        }

        public IEnumerable<Partners> GetPartnersByType(int typeId)
        {
            return _partners.Where(p => p.TypeId == typeId);
        }

        public IEnumerable<Partners> GetPartnersByRating(int minRating, int maxRating)
        {
            return _partners.Where(p => p.Rating >= minRating && p.Rating <= maxRating);
        }

        public IEnumerable<Partners> GetFilteredPartners()
        {
            IEnumerable<Partners> result = _partners;

            if (SelectedTypeId.HasValue && SelectedTypeId.Value > 0)
            {
                result = result.Where(p => p.TypeId == SelectedTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                result = SearchPartners(SearchTerm);
            }

            StatusMessage = $"Найдено партнеров: {result.Count()}";
            return result;
        }

        public IEnumerable<Partners> GetPartnersSortedByName()
        {
            return _partners.OrderBy(p => p.Name);
        }

        public IEnumerable<Partners> GetPartnersSortedByRating()
        {
            return _partners.OrderByDescending(p => p.Rating);
        }

        public IEnumerable<Partners> GetPartnersSortedBySales()
        {
            return _partners
                .Select(p => new { Partner = p, Sales = GetPartnerTotalSales(p.Id) })
                .OrderByDescending(x => x.Sales)
                .Select(x => x.Partner);
        }

        public decimal GetPartnerTotalSales(int partnerId)
        {
            var sales = _salesHistories.Where(s => s.PartnerId == partnerId);
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

        public int CalculatePartnerDiscount(decimal totalSalesAmount)
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

        public int GetPartnerCurrentDiscount(int partnerId)
        {
            var totalSales = GetPartnerTotalSales(partnerId);
            return CalculatePartnerDiscount(totalSales);
        }

        public int GetTotalPartnersCount()
        {
            return _partners.Count;
        }

        public int GetPartnersWithHighRatingCount(int minRating)
        {
            return _partners.Count(p => p.Rating >= minRating);
        }

        public Dictionary<string, int> GetPartnersDistributionByType()
        {
            return _partners
                .GroupBy(p => p.Type?.Name ?? "Без типа")
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public bool IsInnUnique(string inn, int? excludePartnerId = null)
        {
            if (string.IsNullOrEmpty(inn))
                return true;

            var query = _partners.Where(p => p.Inn == inn);

            if (excludePartnerId.HasValue)
            {
                query = query.Where(p => p.Id != excludePartnerId.Value);
            }

            return !query.Any();
        }

        public (bool IsValid, string ErrorMessage) ValidatePartner(Partners partner)
        {
            if (partner == null)
                return (false, "Партнер не может быть null");

            if (partner.TypeId <= 0)
                return (false, "Не выбран тип партнера");

            if (string.IsNullOrWhiteSpace(partner.Name))
                return (false, "Наименование компании обязательно");

            if (string.IsNullOrWhiteSpace(partner.DirectorFullname))
                return (false, "ФИО директора обязательно");

            if (string.IsNullOrWhiteSpace(partner.Phone))
                return (false, "Телефон обязателен");

            if (string.IsNullOrWhiteSpace(partner.Email))
                return (false, "Email обязателен");

            try
            {
                var addr = new System.Net.Mail.MailAddress(partner.Email);
            }
            catch
            {
                return (false, "Некорректный формат email");
            }

            if (partner.Rating < 0)
                return (false, "Рейтинг не может быть отрицательным");

            if (!string.IsNullOrEmpty(partner.Inn))
            {
                if (partner.Inn.Length != 10 && partner.Inn.Length != 12)
                {
                    return (false, "ИНН должен содержать 10 или 12 цифр");
                }

                if (!partner.Inn.All(char.IsDigit))
                {
                    return (false, "ИНН должен содержать только цифры");
                }
            }

            return (true, string.Empty);
        }

        public IEnumerable<Partners> GetPartnersWithExpiredContracts(DateTime referenceDate)
        {
            return _partners.Where(p => p.Rating < 30); 
        }

        public IEnumerable<Partners> GetTopPartnersBySales(int count)
        {
            return GetPartnersSortedBySales().Take(count);
        }

        public IEnumerable<Partners> GetPartnersWithMaxDiscount()
        {
            return _partners.Where(p => GetPartnerCurrentDiscount(p.Id) >= 15);
        }

        public decimal GetAverageSalesAmount()
        {
            if (!_partners.Any())
                return 0;

            var totals = _partners.Select(p => GetPartnerTotalSales(p.Id));
            return totals.Average();
        }
        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                return _context.Product.OrderBy(p => p.Name).ToList();
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Ошибка загрузки продукции: {ex.Message}");
                return new List<Product>();
            }
        }

        public Product FindProductByName(string name)
        {
            try
            {
                return _context.Product
                    .FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Ошибка поиска продукта: {ex.Message}");
                return null;
            }
        }

        public void AddProduct(Product product)
        {
            try
            {
                _context.Product.Add(product);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Ошибка добавления продукта: {ex.Message}");
                throw;
            }
        }
    }
}
