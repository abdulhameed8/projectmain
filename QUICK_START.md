# Quick Start Guide - SaaS Platform API

## What You Have

This project includes:

### 1. **Complete Database Design**
   - 50+ tables covering CRM, CTI, IVR, and Core Banking
   - Multi-tenant architecture
   - Proper relationships and constraints
   - File: `Database_Design_Documentation.md`
   - File: `Database_Creation_Script.sql`

### 2. **Complete .NET Core Web API**
   - Clean Architecture with 4 layers
   - Repository Pattern + Unit of Work
   - FluentValidation for business rules
   - AutoMapper for object mapping
   - Serilog for comprehensive logging
   - Global exception handling
   - Swagger/OpenAPI documentation
   - One complete controller (Customer) as a template

## 5-Minute Setup

### Step 1: Prerequisites
```bash
# Verify .NET 8 is installed
dotnet --version

# Should show 8.0.x or later
```

### Step 2: Create Database
```sql
-- Run this in SQL Server Management Studio or Azure Data Studio
-- Use the Database_Creation_Script.sql file provided
```

### Step 3: Update Connection String
Edit `SaaS.Platform.API/API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=SaaSPlatformDB;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

### Step 4: Run the API
```bash
cd SaaS.Platform.API/API
dotnet restore
dotnet build
dotnet run
```

### Step 5: Test with Swagger
Open browser: `https://localhost:7001`

You'll see the Swagger UI with all API endpoints documented!

## Test the Customer API

### 1. Create a Tenant (Manually in Database first)
```sql
INSERT INTO Tenants (TenantId, TenantName, TenantCode, IsActive)
VALUES (NEWID(), 'Test Company', 'TEST001', 1);
```

### 2. Create a Customer via API

**POST** `https://localhost:7001/api/v1/customers`

```json
{
  "tenantId": "your-tenant-guid-here",
  "customerCode": "CUST001",
  "customerType": "Individual",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+12345678901",
  "mobile": "+12345678901",
  "address": "123 Main Street",
  "city": "New York",
  "state": "NY",
  "country": "USA",
  "postalCode": "10001",
  "customerStatus": "Active",
  "creditLimit": 10000.00
}
```

### 3. Get All Customers

**GET** `https://localhost:7001/api/v1/customers?tenantId=your-tenant-guid&pageNumber=1&pageSize=10`

### 4. Search Customers

**GET** `https://localhost:7001/api/v1/customers/search?tenantId=your-tenant-guid&searchTerm=john`

## Project Structure Overview

```
SaaS.Platform.API/
│
├── Domain/              ← Entities (Customer, User, etc.)
├── Application/         ← DTOs, Validators, Mappings
├── Infrastructure/      ← DbContext, Repositories, Unit of Work
└── API/                 ← Controllers, Middleware, Program.cs
```

## How to Add New Entities

**Example: Adding "Lead" entity**

### 1. Domain Entity
```csharp
// Domain/Entities/Lead.cs
public class Lead : BaseEntity
{
    public Guid LeadId { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    // ... other properties
}
```

### 2. DTOs
```csharp
// Application/DTOs/Lead/LeadDto.cs
public class CreateLeadDto { ... }
public class UpdateLeadDto { ... }
public class LeadDto { ... }
```

### 3. Validator
```csharp
// Application/Validators/LeadValidators.cs
public class CreateLeadDtoValidator : AbstractValidator<CreateLeadDto>
{
    public CreateLeadDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        // Add other rules
    }
}
```

### 4. AutoMapper Profile
```csharp
// Application/Mappings/LeadProfile.cs
public class LeadProfile : Profile
{
    public LeadProfile()
    {
        CreateMap<Lead, LeadDto>();
        CreateMap<CreateLeadDto, Lead>();
    }
}
```

### 5. Repository Interface
```csharp
// Infrastructure/Repositories/Interfaces/ILeadRepository.cs
public interface ILeadRepository : IGenericRepository<Lead>
{
    Task<IEnumerable<Lead>> GetByTenantIdAsync(Guid tenantId);
}
```

### 6. Repository Implementation
```csharp
// Infrastructure/Repositories/LeadRepository.cs
public class LeadRepository : GenericRepository<Lead>, ILeadRepository
{
    public LeadRepository(ApplicationDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Lead>> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet.Where(l => l.TenantId == tenantId).ToListAsync();
    }
}
```

### 7. Add to DbContext
```csharp
// Infrastructure/Data/ApplicationDbContext.cs
public DbSet<Lead> Leads { get; set; }

// In OnModelCreating:
modelBuilder.Entity<Lead>(entity =>
{
    entity.ToTable("Leads");
    entity.HasKey(e => e.LeadId);
    // Configure properties
});
```

### 8. Add to Unit of Work
```csharp
// Infrastructure/UnitOfWork/IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    ICustomerRepository Customers { get; }
    ILeadRepository Leads { get; }  // Add this
    // ...
}

// Infrastructure/UnitOfWork/UnitOfWork.cs
private ILeadRepository? _leadRepository;
public ILeadRepository Leads => _leadRepository ??= new LeadRepository(_context);
```

### 9. Create Controller
```csharp
// API/Controllers/LeadsController.cs
[ApiController]
[Route("api/v1/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<LeadsController> _logger;
    
    // Copy methods from CustomersController and adapt
}
```

## Key Features Demonstrated

✅ **Clean Architecture** - Separation of concerns  
✅ **Repository Pattern** - Abstraction over data access  
✅ **Unit of Work** - Transaction management  
✅ **Dependency Injection** - Loose coupling  
✅ **FluentValidation** - Business rule validation  
✅ **AutoMapper** - Object mapping  
✅ **Serilog** - Comprehensive logging  
✅ **Global Exception Handling** - Consistent error responses  
✅ **API Response Wrapper** - Standardized responses  
✅ **Swagger/OpenAPI** - Interactive documentation  
✅ **Async/Await** - Asynchronous operations  
✅ **Pagination** - Efficient data retrieval  
✅ **Soft Delete** - Data integrity  

## Common Issues & Solutions

### Issue 1: Can't connect to database
**Solution**: 
- Check SQL Server is running
- Verify connection string in appsettings.json
- Use `Trusted_Connection=true` for Windows Authentication
- Or use `User Id=sa;Password=YourPassword` for SQL Authentication

### Issue 2: Port already in use
**Solution**: 
Edit `Properties/launchSettings.json` and change the port numbers

### Issue 3: Migration errors
**Solution**: 
```bash
# Delete the Migrations folder
# Then recreate migrations
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Next Steps

### Phase 1: Core Functionality (Weeks 1-4)
- ✅ Database Design
- ✅ Customer API
- ⬜ User Authentication (JWT)
- ⬜ Lead API
- ⬜ Opportunity API

### Phase 2: Additional Modules (Weeks 5-12)
- ⬜ Banking Module (Accounts, Transactions)
- ⬜ CTI Module (Call Records)
- ⬜ IVR Module (Flows, Sessions)
- ⬜ Workflow Integration (Camunda)

### Phase 3: Advanced Features (Weeks 13-20)
- ⬜ RabbitMQ Integration
- ⬜ File Upload/Download
- ⬜ Email Notifications
- ⬜ Reporting Module
- ⬜ Dashboard APIs

### Phase 4: DevOps & Deployment (Weeks 21-24)
- ⬜ Docker Containerization
- ⬜ CI/CD Pipeline
- ⬜ Cloud Deployment (Azure/AWS)
- ⬜ Monitoring & Logging
- ⬜ Performance Optimization

## Important Files

| File | Purpose |
|------|---------|
| `Database_Design_Documentation.md` | Complete database schema documentation |
| `Database_Creation_Script.sql` | SQL script to create all tables |
| `README.md` | Comprehensive project documentation |
| `CustomersController.cs` | Complete controller example with all CRUD operations |
| `Program.cs` | Application startup and service registration |
| `appsettings.json` | Configuration file |

## Tips for Success

1. **Start Small**: Use the Customer controller as a template for other entities
2. **Follow Patterns**: Maintain consistency with the established patterns
3. **Test as You Go**: Test each endpoint in Swagger before moving to the next
4. **Log Everything**: The logging is already set up - use it!
5. **Read the Code**: The Customer implementation has comments explaining everything

## Getting Help

- Review the `README.md` for detailed documentation
- Check the `Database_Design_Documentation.md` for entity relationships
- Look at `CustomersController.cs` as a reference implementation
- Use Swagger UI to understand API contracts

## Timeline to First Launch

**Target: End of April 2026 (6 months)**

- **Month 1-2**: Core entities (Customer, Lead, Opportunity, User)
- **Month 3**: Banking module (Accounts, Transactions)
- **Month 4**: CTI & IVR modules
- **Month 5**: Workflow integration, Testing
- **Month 6**: Deployment, Marketing preparation

You have a solid foundation - now just replicate the Customer pattern for other entities!

## Success Checklist

- [ ] Database created successfully
- [ ] API running on localhost
- [ ] Swagger UI accessible
- [ ] Can create a customer via API
- [ ] Can retrieve customers with pagination
- [ ] Logs appearing in console and file
- [ ] Validation working (try invalid data)
- [ ] Understanding the project structure
- [ ] Ready to add next entity (Lead/Opportunity)

**If all checkboxes are ticked, you're ready to start building! 🚀**
