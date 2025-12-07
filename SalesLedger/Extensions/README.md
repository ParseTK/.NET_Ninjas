# Extensions
*This folder contains extension methods that enhance the functionality of built-in types, primarily focused on dependency injection configuration.*

## 🎯 Purpose
```
Centralize dependency injection configuration
Keep Program.cs clean and focused
Enable modular service configuration
```

---

## 📁 Contents

**ServiceCollectionExtensions.cs:** *Dependency injection setup for the entire application*

---

## 💡 Usage
In Program.cs:
```CSharp
var builder = WebApplication.CreateBuilder(args);

// Single line registers all services
builder.Services.AddSalesLedgerServices(builder.Configuration);

var app = builder.Build();
app.Run();
```

---

## 🎨 Extension Method Benefits

### Organization

✅ Keeps startup code clean

✅ Groups related registrations

✅ Easy to locate service configuration

### Reusability

✅ Can be used in multiple projects

✅ Testable (can use in test projects)

✅ Shareable across teams

### Maintainability

✅ Single place to modify registrations

✅ Clear dependencies

✅ Easy to add new services

