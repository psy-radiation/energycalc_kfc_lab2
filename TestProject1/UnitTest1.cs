using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;

namespace MyApp.Tests
{
    [TestClass]
    public class BillingServiceTests
    {

        string nonch="mev";
        string ch="mbmvbnmcvbnklm";
        double daylow=59;
        double nightlow=59;

        private readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("http://localhost:5000/") };

        public BillingServiceTests()
        {

        }

        [TestMethod]
        public async Task NewMeter()
        {
            // Arrange
            var jsonContent = new StringContent(
                 $"{{\"MeterId\": \"{ch}\", \"CurrentDay\": 100, \"CurrentNight\": 50}}",
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/meters", jsonContent);
            var respstring = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode(); // Убедитесь, что статус код успешный
            // Assert
            Assert.Contains("Data Added", respstring);
        }

        [TestMethod]
        public async Task OldMeter()
        {
            // Arrange
            var jsonContent = new StringContent(
                $"{{\"MeterId\": \"{nonch}\", \"CurrentDay\": 100, \"CurrentNight\": 50}}",
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/meters", jsonContent);
            var respstring = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode(); // Убедитесь, что статус код успешный

            // Assert
            Assert.Contains("Data Updated", respstring);
        }

        [TestMethod]
        public async Task TestCalculateBilllownight()
        {
            // Arrange
            var jsonContent = new StringContent(
                 $"{{\"MeterId\": \"asd\", \"CurrentDay\": 100, \"CurrentNight\": {nightlow}}}",
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/correct", jsonContent);
            var respstring = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode(); // Убедитесь, что статус код успешный
            nightlow--;
            // Assert
            Assert.Contains("64", respstring);
        }

        [TestMethod]
        public async Task TestCalculateBilllowday()
        {
            // Arrange
            var jsonContent = new StringContent(
                 $"{{\"MeterId\": \"asdf\", \"CurrentDay\": {daylow}, \"CurrentNight\": 100}}",
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/correct", jsonContent);
            var respstring = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode(); // Убедитесь, что статус код успешный
            daylow--;
            // Assert
            Assert.Contains("150", respstring);
        }

        [TestMethod]
        public async Task TestCalculateBilllowall()
        {
            // Arrange
            var jsonContent = new StringContent(
                 $"{{\"MeterId\": \"bnm\", \"CurrentDay\": {daylow}, \"CurrentNight\": {nightlow}}}",
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/correct", jsonContent);
            var respstring = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode(); // Убедитесь, что статус код успешный
            daylow--;
            nightlow--;
            // Assert
            Assert.Contains("214", respstring);
        }

    }
}
