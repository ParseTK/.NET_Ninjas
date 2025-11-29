using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesLedger.Tests
{
    public class ProductTests
        {
        private readonly SqliteConnection _connection;
        private readonly AppDbContext _context;
        private readonly IProductService _service;

        public ProductServiceTests()
        {   
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)   // Use SQLite provider
                .Options;

            _context = new AppDbContext(options);

            _context.Database.EnsureCreated();

            _service = new ProductService(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        // --------------------------------------------------------------
        // CREATE
        // --------------------------------------------------------------
        [Fact]
        public async Task AddAsync_ShouldCreateProduct()
        {
            var product = new Product
            {
                ProductName = "Laptop",
                Price = 800,
                Quantity = 5
            };

            var result = await _service.AddAsync(product);

            Assert.NotNull(result);
            Assert.True(result.ProductId > 0);
        }

        // --------------------------------------------------------------
        // READ ALL
        // --------------------------------------------------------------
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            await _service.AddAsync(new Product { ProductName = "A", Price = 10 });
            await _service.AddAsync(new Product { ProductName = "B", Price = 20 });

            var list = await _service.GetAllAsync();

            Assert.Equal(2, list.Count());
        }

        // --------------------------------------------------------------
        // READ ONE
        // --------------------------------------------------------------
        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct()
        {
            var added = await _service.AddAsync(new Product { ProductName = "Mouse", Price = 15 });

            var found = await _service.GetByIdAsync(added.ProductId);

            Assert.NotNull(found);
            Assert.Equal("Mouse", found.ProductName);
        }

        // --------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------
        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            var added = await _service.AddAsync(new Product { ProductName = "Old", Price = 10 });

            added.ProductName = "New";

            var updated = await _service.UpdateAsync(added);

            Assert.Equal("New", updated.ProductName);
        }

        // --------------------------------------------------------------
        // DELETE
        // --------------------------------------------------------------
        [Fact]
        public async Task DeleteAsync_ShouldDeleteProduct()
        {
            var added = await _service.AddAsync(new Product { ProductName = "DeleteMe", Price = 5 });

            var result = await _service.DeleteAsync(added.ProductId);

            Assert.True(result);

            Assert.Null(await _service.GetByIdAsync(added.ProductId));
        }

        // --------------------------------------------------------------
        // DELETE WITH FK VIOLATION
        // --------------------------------------------------------------
        //
        // This version uses REAL FK rules:
        // Child table references Product.ProductId
        //
        [Fact]
        public async Task DeleteAsync_ShouldThrowFKException_WhenProductIsReferenced()
        {
            
            _context.Database.ExecuteSqlRaw(@"
                CREATE TABLE Orders (
                    OrderId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER NOT NULL,
                    FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE RESTRICT
                );
            ");

            // Add product
            var product = await _service.AddAsync(new Product
            {
                ProductName = "Laptop",
                Price = 900
            });

            // Add referencing order
            _context.Database.ExecuteSqlRaw(
                $"INSERT INTO Orders (ProductId) VALUES ({product.ProductId})");

            // Attempt delete and expect FK violation
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _service.DeleteAsync(product.ProductId);
            });
        }
    }
}
