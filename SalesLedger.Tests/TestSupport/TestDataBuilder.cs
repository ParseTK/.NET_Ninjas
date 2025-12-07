using SalesLedger.Domain;
using SalesLedger.Infrastructure.Data;

namespace SalesLedger.Tests.TestSupport;

public class TestDataBuilder
{
    private readonly SalesLedgerDbContext _context;

    public TestDataBuilder(SalesLedgerDbContext context)
    {
        _context = context;
    }

    #region Customer Builder

    public CustomerBuilder Customer() => new CustomerBuilder(_context);

    public class CustomerBuilder
    {
        private readonly SalesLedgerDbContext _context;
        private Guid _customerId = Guid.NewGuid();
        private string _firstName = "Test";
        private string _lastName = "Customer";
        private string _email = $"customer{Guid.NewGuid():N}@example.com";

        public CustomerBuilder(SalesLedgerDbContext context)
        {
            _context = context;
        }

        public CustomerBuilder WithId(Guid id)
        {
            _customerId = id;
            return this;
        }

        public CustomerBuilder WithName(string firstName, string lastName)
        {
            _firstName = firstName;
            _lastName = lastName;
            return this;
        }

        public CustomerBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public async Task<Customers> BuildAsync()
        {
            var customer = new Customers
            {
                CustomerId = _customerId,
                FirstName = _firstName,
                LastName = _lastName,
                Email = _email
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public Customers Build()
        {
            return new Customers
            {
                CustomerId = _customerId,
                FirstName = _firstName,
                LastName = _lastName,
                Email = _email
            };
        }
    }

    #endregion

    #region Product Builder

    public ProductBuilder Product() => new ProductBuilder(_context);

    public class ProductBuilder
    {
        private readonly SalesLedgerDbContext _context;
        private Guid _productId = Guid.NewGuid();
        private string _name = $"Product {Guid.NewGuid():N}";
        private decimal _price = 99.99m;

        public ProductBuilder(SalesLedgerDbContext context)
        {
            _context = context;
        }

        public ProductBuilder WithId(Guid id)
        {
            _productId = id;
            return this;
        }

        public ProductBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ProductBuilder WithPrice(decimal price)
        {
            _price = price;
            return this;
        }

        public async Task<Products> BuildAsync()
        {
            var product = new Products
            {
                ProductId = _productId,
                Name = _name,
                Price = _price
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public Products Build()
        {
            return new Products
            {
                ProductId = _productId,
                Name = _name,
                Price = _price
            };
        }
    }

    #endregion

    #region Bulk Data Creation Helpers
    public async Task<List<Customers>> CreateCustomersAsync(int count)
    {
        var customers = new List<Customers>();
        for (int i = 0; i < count; i++)
        {
            var customer = await Customer()
                .WithName($"Customer{i}", $"LastName{i}")
                .WithEmail($"customer{i}_{Guid.NewGuid():N}@example.com")
                .BuildAsync();
            customers.Add(customer);
        }
        return customers;
    }
    public async Task<List<Products>> CreateProductsAsync(int count, decimal startPrice = 10m)
    {
        var products = new List<Products>();
        for (int i = 0; i < count; i++)
        {
            var product = await Product()
                .WithName($"Product {i + 1}")
                .WithPrice(startPrice + (i * 10m))
                .BuildAsync();
            products.Add(product);
        }
        return products;
    }

    #endregion

}