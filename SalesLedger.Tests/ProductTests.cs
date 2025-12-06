using Moq;
using SalesLedger.Application.Interfaces;
using SalesLedger.Application.Services;
using SalesLedger.Domain;
using SalesLedger.Infrastructure.Repositories;
using Xunit;

namespace SalesLedger.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock;
        private readonly IProductService _service;

        public ProductServiceTests()
        {
            _repoMock = new Mock<IProductRepository>();
            _repoMock.SetupGet(r => r.UnitOfWork).Returns(new FakeUnitOfWork());
            _service = new ProductService(_repoMock.Object);
        }

        // CREATE
        [Fact]
        public async Task CreateAsync_ShouldPersistProduct()
        {
            var product = new Products { ProductId = Guid.NewGuid(), Name = "Laptop", Price = 800m };

            _repoMock.Setup(r => r.AddAsync(product, It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            var created = await _service.CreateAsync(product);

            Assert.Equal("Laptop", created.Name);
            Assert.Equal(800m, created.Price);
            _repoMock.Verify(r => r.AddAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.CreateAsync(null!)
            );
        }

        // READ
        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
        {
            var id = Guid.NewGuid();
            var product = new Products { ProductId = id, Name = "Mouse", Price = 15m };

            _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(product);

            var found = await _service.GetByIdAsync(id);

            Assert.NotNull(found);
            Assert.Equal("Mouse", found!.Name);
            Assert.Equal(15m, found.Price);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenMissing()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Products?)null);

            var found = await _service.GetByIdAsync(id);

            Assert.Null(found);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            var list = new List<Products>
            {
                new() { ProductId = Guid.NewGuid(), Name = "A", Price = 10m },
                new() { ProductId = Guid.NewGuid(), Name = "B", Price = 20m }
            };

            _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(list);

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        // UPDATE
        [Fact]
        public async Task UpdateAsync_ShouldApplyChanges_WhenExists()
        {
            var id = Guid.NewGuid();
            var existing = new Products { ProductId = id, Name = "Old", Price = 10m };
            var update = new Products { ProductId = id, Name = "New", Price = 20m };

            _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(existing);

            await _service.UpdateAsync(update);

            Assert.Equal("New", existing.Name);
            Assert.Equal(20m, existing.Price);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.UpdateAsync(null!)
            );
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenMissing()
        {
            var id = Guid.NewGuid();
            var update = new Products { ProductId = id, Name = "X", Price = 1m };

            _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Products?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _service.UpdateAsync(update)
            );
        }

        // DELETE
        [Fact]
        public async Task DeleteAsync_ShouldRemove_WhenExists()
        {
            var id = Guid.NewGuid();
            var existing = new Products { ProductId = id, Name = "DeleteMe", Price = 5m };

            _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(existing);

            await _service.DeleteAsync(id);

            _repoMock.Verify(r => r.Remove(existing), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenMissing()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Products?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _service.DeleteAsync(id)
            );
        }

        private sealed class FakeUnitOfWork : IUnitOfWork
        {
            public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
                => Task.FromResult(1);
        }
    }
}
