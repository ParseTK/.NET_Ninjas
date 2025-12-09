using Microsoft.EntityFrameworkCore;
using SalesLedger.Domain;

namespace SalesLedger.Infrastructure.Data.Configurations;

public static class DataSeeding
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        // ================================================================
        // 1. Customers
        // ================================================================
        var johnId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var sarahId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var michaelId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Customers>().HasData(
            new Customers { CustomerId = johnId, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
            new Customers { CustomerId = sarahId, FirstName = "Sarah", LastName = "Smith", Email = "sarah.smith@example.com" },
            new Customers { CustomerId = michaelId, FirstName = "Michael", LastName = "Brown", Email = "michael.brown@example.com" }
        );

        // ================================================================
        // 2. Products
        // ================================================================
        var laptopId = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");
        var mouseId = Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2");
        var keyboardId = Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3");

        modelBuilder.Entity<Products>().HasData(
            new Products { ProductId = laptopId, Name = "Laptop 15-inch", Price = 1200.00m },
            new Products { ProductId = mouseId, Name = "Wireless Mouse", Price = 25.50m },
            new Products { ProductId = keyboardId, Name = "Keyboard", Price = 45.99m }
        );

        // ================================================================
        // 3. Orders
        // ================================================================
        var order1Id = Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4");
        var order2Id = Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5");
        var order3Id = Guid.Parse("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6");

        modelBuilder.Entity<Orders>().HasData(
            new Orders
            {
                OrderId = order1Id,
                CustomerId = johnId,
                OrderDate = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc)
            },
            new Orders
            {
                OrderId = order2Id,
                CustomerId = johnId,
                OrderDate = new DateTime(2025, 1, 11, 0, 0, 0, DateTimeKind.Utc)
            },
            new Orders
            {
                OrderId = order3Id,
                CustomerId = sarahId,
                OrderDate = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // ================================================================
        // 4. OrderItems
        // ================================================================
        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem { OrderId = order1Id, ProductId = laptopId, Quantity = 1, UnitPrice = 1200.00m },
            new OrderItem { OrderId = order2Id, ProductId = mouseId, Quantity = 2, UnitPrice = 25.50m },
            new OrderItem { OrderId = order3Id, ProductId = keyboardId, Quantity = 1, UnitPrice = 45.99m }
        );
    }
}