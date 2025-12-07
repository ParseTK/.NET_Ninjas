# Domain Layer
*The Domain layer contains the core business entities and domain logic. This is the heart of the application and has no dependencies on other layers.*

---

## 🎯 Purpose
- **Defines core business entities**
- **Contains domain logic and business rules**
- **Establishes relationships between entities**
- **Remains independent of infrastructure concerns**
- **Represents the business domain model**

---

## 📁 Contents
- **Customers.cs:** *Customer entity with contact information*
- **Products.cs:** *Product catalog entity with pricing*
- **Orders.cs:** *Order header entity with customer relationship*
- **OrderItem.cs:** *Order line item entity (many-to-many resolution)*

---

## 🏗️ Entity Overview
### Customers
*Represents customers who place orders.*

**Business Rules:**
```
-> Email must be unique across all customers
-> First and last names are required
-> Supports one-to-many relationship with Orders
```

---

### Products
*Represents items available for purchase.*

**Business Rules:**
```
-> Product name is required
-> Price stored with 2 decimal precision
-> Participates in many-to-many relationship with Orders via OrderItem
```

---

### Orders
*Represents a customer's order.*

**Business Rules:**
```
-> Each order belongs to exactly one customer
-> Order date defaults to UTC current time
-> Orders can have multiple order items
```

---

### OrderItem
*Represents a product on an order (many-to-many resolution table).*

**Business Rules:**
```
-> Composite primary key (OrderId + ProductId)
-> UnitPrice captured at order time (may differ from current Product.Price)
-> Quantity must be positive
```

---

## 🔗 Relationships

- **Customers → Orders:** *One-to-Many* (one customer, many orders)
- **Orders → OrderItem:** *One-to-Many* (one order, many order items)
- **Products → OrderItem:** *One-to-Many* (one product, many order items)
- **Orders ↔ Products:** *Many-to-Many* (OrderItem junction table)
