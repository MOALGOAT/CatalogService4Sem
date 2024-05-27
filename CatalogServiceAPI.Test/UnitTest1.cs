using CatalogServiceAPI.Controllers;
using CatalogServiceAPI.Models;
using EnvironmentAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CatalogServiceAPI.Tests
{
    [TestFixture]
    public class CatalogControllerTests
    {
        private Mock<ICatalogInterface> _mockCatalogService;
        private Mock<ILogger<CatalogController>> _mockLogger;
        private CatalogController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCatalogService = new Mock<ICatalogInterface>();
            _mockLogger = new Mock<ILogger<CatalogController>>();
            _controller = new CatalogController(_mockCatalogService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetCatalog_ReturnsOk_WithCatalog() // Opsætter mock'en til at returnere en specifik Catalog, når GetCatalog metoden kaldes.
        {
            // Arrange
            var catalogId = Guid.NewGuid();
            var catalog = new Catalog { _id = catalogId, title = "Test Catalog" };
            _mockCatalogService.Setup(service => service.GetCatalog(catalogId)).ReturnsAsync(catalog);

            // Act
            var result = await _controller.GetCatalog(catalogId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<Catalog>(okResult.Value);
            Assert.AreEqual(catalog, okResult.Value);
        }

        [Test]
        public async Task GetCatalog_ReturnsNotFound_WhenCatalogDoesNotExist() // Opsætter mock'en til at returnere null, når GetCatalog metoden kaldes med en given catalogId.
        {
            // Arrange
            var catalogId = Guid.NewGuid();
            _mockCatalogService.Setup(service => service.GetCatalog(catalogId)).ReturnsAsync((Catalog)null);

            // Act
            var result = await _controller.GetCatalog(catalogId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetCatalogList_ReturnsOk_WithCatalogList() // Opsætter mock'en til at returnere en liste af kataloger, når GetCatalogList metoden kaldes.
        {
            // Arrange
            var catalogList = new List<Catalog> { new Catalog { _id = Guid.NewGuid() } };
            _mockCatalogService.Setup(service => service.GetCatalogList()).ReturnsAsync(catalogList);

            // Act
            var result = await _controller.GetCatalogList();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<IEnumerable<Catalog>>(okResult.Value);
            Assert.AreEqual(catalogList, okResult.Value);
        }

        [Test]
        public async Task AddCatalog_ReturnsCreated_WithValidCatalog() // Opsætter mock'en til at returnere en ny catalogId, når AddCatalog metoden kaldes.
        {
            // Arrange
            var catalog = new Catalog { title = "New Catalog" };
            var catalogId = Guid.NewGuid();
            _mockCatalogService.Setup(service => service.AddCatalog(catalog)).ReturnsAsync(catalogId);

            // Act
            var result = await _controller.AddCatalog(catalog);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.AreEqual(nameof(_controller.GetCatalog), createdResult.ActionName);
            Assert.AreEqual(catalogId, createdResult.RouteValues["_id"]);
        }

        [Test]
        public async Task AddCatalog_ReturnsBadRequest_WhenCatalogIsNull() // Tester, hvad der sker, når en null værdi sendes som parameter til AddCatalog metoden.
        {
            // Arrange
            Catalog catalog = null;

            // Act
            var result = await _controller.AddCatalog(catalog);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
        }

        [Test]
        public async Task UpdateCatalog_ReturnsNoContent_WhenSuccessful() // Opsætter mock'en til at returnere 1 (en ændring), når UpdateCatalog metoden kaldes.
        {
            // Arrange
            var catalogId = Guid.NewGuid();
            var catalog = new Catalog { _id = catalogId, title = "Updated Catalog" };
            _mockCatalogService.Setup(service => service.UpdateCatalog(catalog)).ReturnsAsync(1);

            // Act
            var result = await _controller.UpdateCatalog(catalogId, catalog);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateCatalog_ReturnsBadRequest_WhenCatalogIdMismatch() // Tester, hvad der sker, når ID i URL'en ikke matcher ID i kataloget.

        {
            // Arrange
            var catalogId = Guid.NewGuid();
            var catalog = new Catalog { _id = Guid.NewGuid() };

            // Act
            var result = await _controller.UpdateCatalog(catalogId, catalog);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task DeleteCatalog_ReturnsOk_WhenSuccessful() // Opsætter mock'en til at returnere 1 (en sletning), når DeleteCatalog metoden kaldes.
        {
            // Arrange
            var catalogId = Guid.NewGuid();
            _mockCatalogService.Setup(service => service.DeleteCatalog(catalogId)).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteCatalog(catalogId);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task DeleteCatalog_ReturnsNotFound_WhenCatalogDoesNotExist() // Opsætter mock'en til at returnere 0 (ingen sletning), når DeleteCatalog metoden kaldes med en given catalogId.
        {
            // Arrange
            var catalogId = Guid.NewGuid();
            _mockCatalogService.Setup(service => service.DeleteCatalog(catalogId)).ReturnsAsync(0);

            // Act
            var result = await _controller.DeleteCatalog(catalogId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
