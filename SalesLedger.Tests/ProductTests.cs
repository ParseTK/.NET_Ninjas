using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SalesLedger.Application.Interfaces;
using SalesLedger.Application.Services;
using SalesLedger.Data;
using SalesLedger.Models;
using SalesLedger.Tests.TestSupport;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                await __service.CreateAsync(null);
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

        // ------------------------ NEW TESTS ------------------------

        [Fact]
        public async Task CreateAsync_ShouldTrimProductName()
        {
            var product = new Products
            {
                ProductName = "   Trimmed Name   ",
                UnitPrice = 200m
            };

            var created = await _service.CreateAsync(product);

            Assert.Equal("Trimmed Name", created.ProductName);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenNameIsWhitespace()
        {
            var product = new Products
            {
                ProductName = "    ",
                UnitPrice = 10m
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.CreateAsync(product);
            });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenUnitPriceIsVeryLarge()
        {
            var product = new Products
            {
                ProductName = "LargePrice",
                UnitPrice = decimal.MaxValue
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.CreateAsync(product);
            });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenUnitPriceIsZero()
        {
            var product = new Products
            {
                ProductName = "ZeroPrice",
                UnitPrice = 0m
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.CreateAsync(product);
            });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenProductNameAlreadyExists()
        {
            await _service.CreateAsync(new Products
            {
                ProductName = "UniqueName",
                UnitPrice = 50m
            });

            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _service.CreateAsync(new Products
                {
                    ProductName = "UniqueName",
                    UnitPrice = 60m
                });
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

        // ------------------------ NEW TESTS ------------------------

        [Fact]
        public async Task GetAllAsync_ShouldNotReturnSoftDeletedProducts()
        {
            var p = await _service.CreateAsync(new Products
            {
                ProductName = "SoftDelete",
                UnitPrice = 50m
            });

            p.IsDeleted = true;
            Context.Products.Update(p);
            await Context.SaveChangesAsync();

            var list = await _service.GetAllAsync();

            Assert.DoesNotContain(list, x => x.ProductId == p.ProductId);
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
            Assert.Equal("Mouse", found!.ProductName);
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

        // ------------------------ NEW TESTS ------------------------

        [Fact]
        public async Task GetByIdAsync_ShouldHandleAnyGuid()
        {
            var result = await _service.GetByIdAsync(Guid.NewGuid());
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

        // ------------------------ NEW TESTS ------------------------

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenUpdatingToDuplicateName()
        {
            var p1 = await _service.CreateAsync(new Products
            {
                ProductName = "Product1",
                UnitPrice = 10m
            });

            var p2 = await _service.CreateAsync(new Products
            {
                ProductName = "Product2",
                UnitPrice = 20m
            });

            p2.ProductName = "Product1";

            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _service.UpdateAsync(p2);
            });
        }

        [Fact]
        public async Task UpdateAsync_ShouldTrimProductName()
        {
            var added = await _service.CreateAsync(new Products
            {
                ProductName = "TrimTest",
                UnitPrice = 10m
            });

            added.ProductName = "   NewTrimmedName   ";

            var result = await _service.UpdateAsync(added);
            Assert.True(result);

            var updated = await _service.GetByIdAsync(added.ProductId);
            Assert.Equal("NewTrimmedName", updated.ProductName);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenSettingPriceToVeryLargeValue()
        {
            var added = await _service.CreateAsync(new Products
            {
                ProductName = "LargeUpdate",
                UnitPrice = 20m
            });

            added.UnitPrice = decimal.MaxValue;

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

        // ------------------------ NEW TESTS ------------------------

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenSoftDeleted()
        {
            var added = await _service.CreateAsync(new Products { ProductName = "SoftDel", UnitPrice = 20m });

            added.IsDeleted = true;
            Context.Products.Update(added);
            await Context.SaveChangesAsync();

            var result = await _service.DeleteAsync(added.ProductId);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenIdFormatIsValidButNonexistent()
        {
            var fakeId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var result = await _service.DeleteAsync(fakeId);

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
