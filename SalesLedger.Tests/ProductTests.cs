using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SalesLedger.Data;
using SalesLedger.Interfaces;
using SalesLedger.Models;
using SalesLedger.Services;
using SalesLedger.Tests.TestSupport;
using Xunit;

namespace SalesLedger.Tests
{
    public class ProductServiceTests : DatabaseTestBase
    {
        private readonly IProductService _service;
        private readonly Mock<ILogger<ProductService>> _loggerMock;

        public ProductServiceTests()
        {
            _loggerMock = CreateMockLogger<ProductService>();
            _service = new ProductService(Context, _loggerMock.Object);
        }

        // --------------------------------------------------------------
        // CREATE
        // --------------------------------------------------------------
        [Fact]
        public async Task CreateAsync_ShouldCreateProduct()
        {
            var product = new Products
            {
                ProductName = "Laptop",
                UnitPrice = 800.00m
            };

            var result = await _service.CreateAsync(product);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.ProductId);
            Assert.Equal("Laptop", result.ProductName);
            Assert.Equal(800.00m, result.UnitPrice);
        }

        [Fact]
        public async Task CreateAsync_ShouldGenerateGuidIfEmpty()
        {
            var product = new Products
            {
                ProductId = Guid.Empty,
                ProductName = "Mouse",
                UnitPrice = 25.00m
            };

            var result = await _service.CreateAsync(product);

            Assert.NotEqual(Guid.Empty, result.ProductId);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenProductIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                
                await _service.CreateAsync(null);

            });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenProductNameIsEmpty()
        {
            var product = new Products
            {
                ProductName = "",
                UnitPrice = 100m
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.CreateAsync(product);
            });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenPriceIsNegative()
        {
            var product = new Products
            {
                ProductName = "Test Product",
                UnitPrice = -10m
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.CreateAsync(product);
            });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenProductNameExceeds200Characters()
        {
            var product = new Products
            {
                ProductName = new string('A', 201),
                UnitPrice = 100m
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.CreateAsync(product);
            });
        }

        // --------------------------------------------------------------
        // READ ALL
        // --------------------------------------------------------------
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            await _service.CreateAsync(new Products { ProductName = "Product A", UnitPrice = 10.00m });
            await _service.CreateAsync(new Products { ProductName = "Product B", UnitPrice = 20.00m });

            var list = await _service.GetAllAsync();

            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProductsInAlphabeticalOrder()
        {
            await _service.CreateAsync(new Products { ProductName = "Zebra", UnitPrice = 10.00m });
            await _service.CreateAsync(new Products { ProductName = "Apple", UnitPrice = 20.00m });
            await _service.CreateAsync(new Products { ProductName = "Mango", UnitPrice = 15.00m });

            var list = (await _service.GetAllAsync()).ToList();

            Assert.Equal("Apple", list[0].ProductName);
            Assert.Equal("Mango", list[1].ProductName);
            Assert.Equal("Zebra", list[2].ProductName);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoProducts()
        {
            var list = await _service.GetAllAsync();

            Assert.Empty(list);
        }

        // --------------------------------------------------------------
        // READ ONE
        // --------------------------------------------------------------
        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct()
        {
            var added = await _service.CreateAsync(new Products { ProductName = "Mouse", UnitPrice = 15.00m });

            var found = await _service.GetByIdAsync(added.ProductId);

            Assert.NotNull(found);
            Assert.Equal("Mouse", found!.ProductName); // ✅ null-forgiving operator
            Assert.Equal(15.00m, found.UnitPrice);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductNotFound()
        {
            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenIdIsEmpty()
        {
            var result = await _service.GetByIdAsync(Guid.Empty);

            Assert.Null(result);
        }

        // --------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------
        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            var added = await _service.CreateAsync(new Products { ProductName = "Old Name", UnitPrice = 10.00m });

            added.ProductName = "New Name";
            added.UnitPrice = 20.00m;

            var result = await _service.UpdateAsync(added);

            Assert.True(result);

            var updated = await _service.GetByIdAsync(added.ProductId);
            Assert.NotNull(updated);
            Assert.Equal("New Name", updated!.ProductName);
            Assert.Equal(20.00m, updated.UnitPrice);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenProductNotFound()
        {
            var product = new Products
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Non-existent",
                UnitPrice = 10.00m
            };

            var result = await _service.UpdateAsync(product);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenProductIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                
                await _service.UpdateAsync(null);
            });
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenIdIsEmpty()
        {
            var product = new Products
            {
                ProductId = Guid.Empty,
                ProductName = "Test",
                UnitPrice = 10.00m
            };

            var result = await _service.UpdateAsync(product);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenValidationFails()
        {
            var added = await _service.CreateAsync(new Products { ProductName = "Valid", UnitPrice = 10.00m });

            added.ProductName = ""; // Invalid

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.UpdateAsync(added);
            });
        }

        // --------------------------------------------------------------
        // DELETE
        // --------------------------------------------------------------
        [Fact]
        public async Task DeleteAsync_ShouldDeleteProduct()
        {
            var added = await _service.CreateAsync(new Products { ProductName = "DeleteMe", UnitPrice = 5.00m });

            var result = await _service.DeleteAsync(added.ProductId);

            Assert.True(result);
            Assert.Null(await _service.GetByIdAsync(added.ProductId));
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenProductNotFound()
        {
            var result = await _service.DeleteAsync(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenIdIsEmpty()
        {
            var result = await _service.DeleteAsync(Guid.Empty);

            Assert.False(result);
        }

        // --------------------------------------------------------------
        // DELETE WITH FK VIOLATION
        // --------------------------------------------------------------
        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenProductHasOrders()
        {
            var product = await _service.CreateAsync(new Products
            {
                ProductName = "Laptop",
                UnitPrice = 900.00m
            });

            var order = new Orders
            {
                OrderId = Guid.NewGuid(),
                ProductId = product.ProductId,
                Quantity = 2,
                UnitPrice = 1800.00m,
                OrderDate = DateTime.UtcNow
            };
            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _service.DeleteAsync(product.ProductId);
            });

            var stillExists = await _service.GetByIdAsync(product.ProductId);
            Assert.NotNull(stillExists);
        }
    }
}
