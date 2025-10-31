# SaaS Platform API - .NET Core Web API Project

## Overview
This is a comprehensive .NET Core 8.0 Web API project demonstrating best practices for building a scalable SaaS platform with CRM, CTI, IVR, and Core Banking modules.

## Project Structure

```
SaaS.Platform.API/
├── Domain/                          # Domain Layer
│   ├── Common/
│   │   └── BaseEntity.cs           # Base entity with audit fields
│   └── Entities/
│       └── Customer.cs             # Customer entity
│
├── Application/                     # Application Layer
│   ├── Common/
│   │   └── ApiResponse.cs          # API response wrappers
│   ├── DTOs/
│   │   └── Customer/
│   │       └── CustomerDto.cs      # Customer DTOs (Create, Update, Response)
│   ├── Mappings/
│   │   └── CustomerProfile.cs      # AutoMapper profiles
│   └── Validators/
│       └── CustomerValidators.cs   # FluentValidation validators
│
├── Infrastructure/                  # Infrastructure Layer
│   ├── Data/
│   │   └── ApplicationDbContext.cs # EF Core DbContext
│   ├── Repositories/
│   │   ├── Interfaces/
│   │   │   ├── IGenericRepository.cs
│   │   │   └── ICustomerRepository.cs
│   │   ├── GenericRepository.cs    # Generic CRUD operations
│   │   └── CustomerRepository.cs   # Customer-specific operations
│   └── UnitOfWork/
│       ├── IUnitOfWork.cs
│       └── UnitOfWork.cs           # Transaction management
│
└── API/                            # Presentation Layer
    ├── Controllers/
    │   └── CustomersController.cs  # Customer API endpoints
    ├── Middleware/
    │   └── ExceptionHandlingMiddleware.cs # Global exception handler
    ├── Program.cs                  # Application entry point
    ├── appsettings.json           # Configuration
    └── SaaS.Platform.API.csproj   # Project file
```

## Key Features

### 1. **Clean Architecture**
- Separation of concerns with Domain, Application, Infrastructure, and API layers
- Dependency Injection throughout the application

### 2. **Repository Pattern & Unit of Work**
- Generic repository for common CRUD operations
- Specific repositories for entity-specific logic
- Unit of Work for transaction management

### 3. **Validation**
- FluentValidation for comprehensive business rule validation
- Separate validators for Create and Update operations
- Detailed error messages

### 4. **Logging**
- Serilog integration with multiple sinks (Console, File)
- Structured logging with context enrichment
- Request/response logging

### 5. **Exception Handling**
- Global exception handling middleware
- Consistent error responses
- Proper HTTP status codes

### 6. **API Response Standardization**
- Consistent response wrapper for all API responses
- Paginated response support
- Error handling with detailed messages

### 7. **AutoMapper**
- Entity to DTO mappings
- Reduced boilerplate code

### 8. **Swagger/OpenAPI**
- Interactive API documentation
- Request/response examples
- JWT Bearer authentication support

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server (2019 or later) or SQL Server Express
- Visual Studio 2022 / VS Code / Rider

## Getting Started

### 1. Clone or Create the Project

If using the files provided, ensure the folder structure matches the one shown above.

### 2. Update Connection String

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=SaaSPlatformDB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Create Database

#### Option A: Run the SQL Script
Execute the `Database_Creation_Script.sql` file in SQL Server Management Studio or Azure Data Studio.

#### Option B: Use EF Core Migrations (Recommended)

```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Navigate to the API project folder
cd SaaS.Platform.API/API

# Add initial migration
dotnet ef migrations add InitialCreate --project ../Infrastructure --startup-project .

# Update database
dotnet ef database update --project ../Infrastructure --startup-project .
```

### 4. Restore NuGet Packages

```bash
dotnet restore
```

### 5. Build the Project

```bash
dotnet build
```

### 6. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5001`
- Swagger UI: `https://localhost:7001` (root URL in development)

## API Endpoints

### Customer Management

#### Get All Customers (Paginated)
```http
GET /api/v1/customers?tenantId={guid}&pageNumber=1&pageSize=10
```

Optional query parameters:
- `searchTerm`: Search in name, email, customer code
- `status`: Filter by customer status
- `segment`: Filter by customer segment

#### Get Customer by ID
```http
GET /api/v1/customers/{id}
```

#### Get Customer by Code
```http
GET /api/v1/customers/by-code/{customerCode}?tenantId={guid}
```

#### Create Customer
```http
POST /api/v1/customers
Content-Type: application/json

{
  "tenantId": "guid",
  "customerCode": "CUST001",
  "customerType": "Individual",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "mobile": "+1234567890",
  "address": "123 Main St",
  "city": "New York",
  "state": "NY",
  "country": "USA",
  "postalCode": "10001",
  "customerStatus": "Active",
  "creditLimit": 5000.00
}
```

#### Update Customer
```http
PUT /api/v1/customers/{id}
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com",
  "phone": "+1234567890"
}
```

#### Delete Customer (Soft Delete)
```http
DELETE /api/v1/customers/{id}
```

#### Search Customers
```http
GET /api/v1/customers/search?tenantId={guid}&searchTerm=john
```

#### Get Active Customers
```http
GET /api/v1/customers/active?tenantId={guid}
```

## Response Format

### Success Response
```json
{
  "success": true,
  "message": "Customer created successfully",
  "data": {
    "customerId": "guid",
    "customerCode": "CUST001",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    ...
  },
  "errors": [],
  "timestamp": "2024-10-31T12:00:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "Email is required",
    "Customer code must contain only uppercase letters, numbers, and hyphens"
  ],
  "timestamp": "2024-10-31T12:00:00Z"
}
```

### Paginated Response
```json
{
  "success": true,
  "message": "Data retrieved successfully",
  "data": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalRecords": 50,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "timestamp": "2024-10-31T12:00:00Z"
}
```

## Validation Rules

### Customer Validation

#### Create Customer:
- **Customer Code**: Required, max 50 chars, uppercase letters/numbers/hyphens only
- **Customer Type**: Required, must be "Individual" or "Corporate"
- **First Name**: Required for Individual customers, max 100 chars
- **Last Name**: Required for Individual customers, max 100 chars
- **Company Name**: Required for Corporate customers, max 200 chars
- **Email**: Required, valid email format, max 255 chars
- **Phone/Mobile**: Valid international phone format
- **Credit Limit**: Cannot be negative
- **Credit Score**: Between 300 and 850
- **Date of Birth**: Must be in the past

#### Update Customer:
- All fields optional
- Same validation rules apply when field is provided

## Best Practices Demonstrated

1. **Repository Pattern**: Abstraction over data access logic
2. **Unit of Work**: Transaction management and coordination
3. **Dependency Injection**: Loose coupling and testability
4. **DTO Pattern**: Separation of internal and external models
5. **Validation**: Business rules enforced with FluentValidation
6. **Logging**: Comprehensive logging with Serilog
7. **Exception Handling**: Centralized error handling
8. **API Versioning**: URL-based versioning
9. **Soft Delete**: Maintain data integrity
10. **Audit Fields**: Track creation and modification

## Extending the API

### Adding a New Entity

1. **Create Domain Entity**
   ```csharp
   // Domain/Entities/YourEntity.cs
   public class YourEntity : BaseEntity
   {
       public Guid Id { get; set; }
       public Guid TenantId { get; set; }
       // Add properties
   }
   ```

2. **Create DTOs**
   ```csharp
   // Application/DTOs/YourEntity/YourEntityDto.cs
   public class CreateYourEntityDto { }
   public class UpdateYourEntityDto { }
   public class YourEntityDto { }
   ```

3. **Create Validators**
   ```csharp
   // Application/Validators/YourEntityValidators.cs
   public class CreateYourEntityDtoValidator : AbstractValidator<CreateYourEntityDto> { }
   ```

4. **Create AutoMapper Profile**
   ```csharp
   // Application/Mappings/YourEntityProfile.cs
   public class YourEntityProfile : Profile { }
   ```

5. **Create Repository**
   ```csharp
   // Infrastructure/Repositories/Interfaces/IYourEntityRepository.cs
   public interface IYourEntityRepository : IGenericRepository<YourEntity> { }
   
   // Infrastructure/Repositories/YourEntityRepository.cs
   public class YourEntityRepository : GenericRepository<YourEntity>, IYourEntityRepository { }
   ```

6. **Add to DbContext**
   ```csharp
   // Infrastructure/Data/ApplicationDbContext.cs
   public DbSet<YourEntity> YourEntities { get; set; }
   ```

7. **Add to Unit of Work**
   ```csharp
   // Infrastructure/UnitOfWork/IUnitOfWork.cs & UnitOfWork.cs
   IYourEntityRepository YourEntities { get; }
   ```

8. **Create Controller**
   ```csharp
   // API/Controllers/YourEntitiesController.cs
   [ApiController]
   [Route("api/v1/[controller]")]
   public class YourEntitiesController : ControllerBase { }
   ```

## Configuration

### appsettings.json

- **ConnectionStrings**: Database connection
- **Serilog**: Logging configuration
- **AllowedOrigins**: CORS configuration
- **JwtSettings**: JWT authentication (for future implementation)

### Environment-Specific Settings

Create `appsettings.Development.json` and `appsettings.Production.json` for environment-specific configurations.

## Security Considerations

1. **Input Validation**: Always validate input using FluentValidation
2. **SQL Injection**: Use parameterized queries (EF Core handles this)
3. **Connection Strings**: Store in secure configuration (Azure Key Vault, AWS Secrets Manager)
4. **JWT Secret Keys**: Use strong, randomly generated keys
5. **HTTPS**: Always use HTTPS in production
6. **CORS**: Configure strict CORS policies for production

## Performance Optimization

1. **Pagination**: Always use pagination for large datasets
2. **Async/Await**: All database operations are asynchronous
3. **Indexes**: Database indexes on frequently queried columns
4. **Connection Pooling**: Enabled by default in EF Core
5. **Caching**: Consider adding Redis for frequently accessed data

## Testing

### Unit Tests
Create unit tests for:
- Validators
- Repository methods
- Business logic

### Integration Tests
Test API endpoints with:
- WebApplicationFactory
- In-memory database

## Logging

Logs are written to:
- **Console**: For development
- **File**: `Logs/log-YYYYMMDD.txt` (rotating daily)

Log levels:
- **Trace**: Very detailed, typically only enabled during development
- **Debug**: Debugging information
- **Information**: General informational messages
- **Warning**: Warning messages for potentially harmful situations
- **Error**: Error messages for serious problems
- **Fatal**: Very severe error events

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database user has proper permissions

### Migration Issues
- Delete Migrations folder and re-create
- Ensure correct project paths in migration commands

### Port Already in Use
- Change ports in `Properties/launchSettings.json`

## Next Steps

1. **Authentication & Authorization**
   - Implement JWT authentication
   - Add role-based authorization
   - Implement refresh tokens

2. **Additional Modules**
   - Implement other entities (Leads, Opportunities, Transactions, etc.)
   - Add more business logic

3. **Advanced Features**
   - Implement Camunda BPMN integration
   - Add RabbitMQ messaging
   - Implement file upload/download
   - Add email notifications

4. **Testing**
   - Write unit tests
   - Write integration tests
   - Add automated testing pipeline

5. **Deployment**
   - Containerize with Docker
   - Deploy to Azure/AWS
   - Set up CI/CD pipeline

## License

This project is for demonstration purposes.

## Support

For questions or issues, please refer to the documentation or create an issue in the repository.
