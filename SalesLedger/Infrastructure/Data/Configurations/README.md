# Infrastructure - Configurations
*This folder contains Entity Framework Core Fluent API configurations for each domain entity. Configurations define table mappings, column constraints, relationships, and indexes.*

---

## 🎯 Purpose

- Keep domain entities clean (no EF attributes)
- Provide type-safe, compile-time checked mappings
- Centralize database schema definition
- Define relationships and constraints explicitly
- Configure indexes and keys

---

## 📁 Contents

- **CustomerConfiguration.cs:** *Customers entity mapping*
- **ProductConfiguration.cs:** *Products entity mapping*
- **OrderConfiguration.cs:** *Orders entity mapping*
- **OrderItemConfiguration.cs:** *OrderItem entity mapping with composite key*

---

## 📊 Configuration Example

**CustomerConfiguration**

<details> <summary> Click to Expand </summary>
  
```csharp
public class CustomerConfiguration : IEntityTypeConfiguration<Customers>
{
    public void Configure(EntityTypeBuilder<Customers> builder)
    {
        // Primary Key
        builder.HasKey(c => c.CustomerId);

        // Properties
        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(255);

        // Unique Index
        builder.HasIndex(c => c.Email)
            .IsUnique();

        // Relationship: Customer -> Orders (1:Many)
        builder.HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```
</details>

---

## 🚨 Delete Behaviors

- **Cascade:** *Delete related entities (e.g., deleting customer deletes orders)*
- **Restrict:** *Prevent deletion if related entities exist*
- **SetNull:** *Set foreign key to null when principal is deleted*
- **NoAction:** *No automatic action (prevents cascade conflicts)*

---

## ✅ Best Practices

- **Explicit Configuration:** *Don't rely on conventions, be explicit*
- **Separate Files:** *One configuration per entity*
- **No Business Logic:** *Configurations are for schema only*
- **Fluent API Over Attributes:** *Keep domain entities clean*
- **Consistent Naming:** *Match database conventions*
- **Index Foreign Keys:** *Improve query performance*
- **Appropriate Delete Behavior:** *Prevent cascade conflicts*
