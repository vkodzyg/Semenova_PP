using Microsoft.VisualStudio.TestTools.UnitTesting;
using semenova_library;
using System;
using System.Diagnostics;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass()]
    public class UnitTest1
    {
     
        [TestMethod()]
        public void CalculatePartnerDiscount_ShouldReturnCorrectValue()
        {
            MainApp app = new MainApp();

            Assert.AreEqual(0, app.CalculatePartnerDiscount(5000));
            Assert.AreEqual(5, app.CalculatePartnerDiscount(25000));
            Assert.AreEqual(10, app.CalculatePartnerDiscount(100000));
            Assert.AreEqual(15, app.CalculatePartnerDiscount(350000));
        }

        [TestMethod()]
        public void ValidatePartner_ValidPartner_ReturnsTrue()
        {
            MainApp app = new MainApp();

            Partners partner = new Partners
            {
                TypeId = 1,
                Name = "Тест",
                DirectorFullname = "Иванов",
                Phone = "123",
                Email = "test@test.com",
                Rating = 10
            };

            var result = app.ValidatePartner(partner);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod()]
        public void ValidatePartner_InvalidEmail_ReturnsFalse()
        {
            MainApp app = new MainApp();

            Partners partner = new Partners
            {
                TypeId = 1,
                Name = "Тест",
                DirectorFullname = "Иванов",
                Phone = "123",
                Email = "invalid-email",
                Rating = 10
            };

            var result = app.ValidatePartner(partner);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod()]
        public void ValidatePartner_EmptyName_ReturnsFalse()
        {
            MainApp app = new MainApp();

            Partners partner = new Partners
            {
                TypeId = 1,
                Name = "",
                DirectorFullname = "Иванов",
                Phone = "123",
                Email = "test@test.com",
                Rating = 10
            };

            var result = app.ValidatePartner(partner);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod()]
        public void ValidatePartner_NegativeRating_ReturnsFalse()
        {
            MainApp app = new MainApp();

            Partners partner = new Partners
            {
                TypeId = 1,
                Name = "Тест",
                DirectorFullname = "Иванов",
                Phone = "123",
                Email = "test@test.com",
                Rating = -5
            };

            var result = app.ValidatePartner(partner);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod()]
        public void ValidatePartner_InnWithLetters_ReturnsFalse()
        {
            MainApp app = new MainApp();

            Partners partner = new Partners
            {
                TypeId = 1,
                Name = "Тест",
                DirectorFullname = "Иванов",
                Phone = "123",
                Email = "test@test.com",
                Rating = 10,
                Inn = "1234AB7890"
            };

            var result = app.ValidatePartner(partner);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod()]
        public void ValidatePartner_ValidInn_ReturnsTrue()
        {
            MainApp app = new MainApp();

            Partners partner = new Partners
            {
                TypeId = 1,
                Name = "Тест",
                DirectorFullname = "Иванов",
                Phone = "123",
                Email = "test@test.com",
                Rating = 10,
                Inn = "1234567890"
            };

            var result = app.ValidatePartner(partner);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod()]
        public void CalculatePartnerDiscount_EdgeCases_WorksCorrectly()
        {
            MainApp app = new MainApp();

            Assert.AreEqual(0, app.CalculatePartnerDiscount(9999));
            Assert.AreEqual(5, app.CalculatePartnerDiscount(10000));
            Assert.AreEqual(5, app.CalculatePartnerDiscount(49999));
            Assert.AreEqual(10, app.CalculatePartnerDiscount(50000));
            Assert.AreEqual(10, app.CalculatePartnerDiscount(299999));
            Assert.AreEqual(15, app.CalculatePartnerDiscount(300000));
        }

  
    }
}
