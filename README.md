# API Versioning Playground

A comprehensive demonstration of API versioning strategies in ASP.NET Core using both **Controller-based** and **Minimal API** approaches. This project showcases best practices for implementing API versioning using the `Asp.Versioning` library.

## 📋 Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [API Versioning Strategies](#api-versioning-strategies)
- [Implementation Approaches](#implementation-approaches)
  - [Controller-Based API](#controller-based-api)
  - [Minimal API](#minimal-api)
- [Getting Started](#getting-started)
- [Testing the APIs](#testing-the-apis)
- [Key Differences Between Versions](#key-differences-between-versions)
- [Best Practices](#best-practices)
- [Additional Resources](#additional-resources)

## 🎯 Overview

API versioning is crucial for maintaining backward compatibility while evolving your APIs. This playground demonstrates:

- **URL Segment Versioning**: Using version numbers in the URL path (`/api/v1/products`, `/api/v2/products`)
- **Multiple API Versions**: Supporting both v1.0 and v2.0 simultaneously
- **Different Implementation Patterns**: Controller-based vs Minimal API approaches
- **OpenAPI Integration**: Swagger documentation with version-aware endpoints

## 📁 Project Structure

```
apiVersioningPlayground/
├── apiVersioningPlayground.sln          # Solution file
├── global.json                          # .NET SDK version configuration
├── README.md                           # This documentation
├── controllerBasedAPI/                 # Traditional controller approach
│   ├── Controllers/
│   │   └── ProductController.cs        # Versioned controller endpoints
│   ├── Program.cs                      # Startup configuration
│   ├── controllerBasedAPI.csproj      # Project dependencies
│   └── controllerBasedAPI.http        # HTTP test requests
└── minimalAPI/                        # Modern minimal API approach
    ├── Program.cs                      # All configuration and endpoints
    ├── minimalAPI.csproj              # Project dependencies
    └── minimalAPI.http                # HTTP test requests
```

## 🔄 API Versioning Strategies

This project implements **URL Segment Versioning**, where the version is embedded in the URL path:

```
GET /api/v1/products    # Version 1.0
GET /api/v2/products    # Version 2.0
```

### Why URL Segment Versioning?

- ✅ **Clear and Explicit**: Version is immediately visible in the URL
- ✅ **Cache-Friendly**: Different URLs can be cached independently
- ✅ **Easy Testing**: Simple to test different versions with tools like Postman
- ✅ **Documentation-Friendly**: Clear separation in API documentation

## 🏗️ Implementation Approaches

### Controller-Based API

The traditional ASP.NET Core approach using controllers and attributes.

#### Configuration (`Program.cs`)

```csharp
// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader()
    );
}).AddMvc();
```

#### Controller Implementation

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [ApiVersion("1.0")]
    public IActionResult GetProductsV1() { /* ... */ }

    [HttpGet]
    [ApiVersion("2.0")]
    public IActionResult GetProductsV2() { /* ... */ }
}
```

#### Key Features:
- Uses `[ApiVersion]` attributes to specify supported versions
- Route template includes `{version:apiVersion}` placeholder
- Separate methods for different versions
- Rich metadata support with `[EndpointSummary]` and `[Tags]`

### Minimal API

The modern, streamlined approach using endpoint routing.

#### Configuration (`Program.cs`)

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader()
    );
});
```

#### Endpoint Registration

```csharp
// Create API version set
var versionSet = app.NewApiVersionSet("Products API")
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .Build();

// Version 1 endpoints
var v1Group = app.MapGroup("/api/v{version:apiVersion}/products")
    .WithTags("Products v1");

v1Group.MapGet("", GetProductsV1)
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(1, 0));
```

#### Key Features:
- Uses `NewApiVersionSet()` to define supported versions
- Endpoint groups with `MapGroup()` for organization
- Fluent API for configuration with `WithApiVersionSet()`
- Static methods as endpoint handlers

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Your favorite IDE (Visual Studio, VS Code, Rider)

### Running the Projects

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd apiVersioningPlayground
   ```

2. **Run the Controller-Based API**
   ```bash
   cd controllerBasedAPI
   dotnet run
   ```
   API will be available at: `https://localhost:5163` or `http://localhost:5163`

3. **Run the Minimal API** (in a separate terminal)
   ```bash
   cd minimalAPI
   dotnet run
   ```
   API will be available at: `https://localhost:5050` or `http://localhost:5050`

4. **View OpenAPI Documentation**
   - Controller API: `https://localhost:5163/openapi/v1.json`
   - Minimal API: `https://localhost:5050/openapi/v1.json`

## 🧪 Testing the APIs

Both projects include `.http` files for easy testing with VS Code REST Client extension or similar tools.

### Example Requests

#### Version 1 (Simple Format)
```http
GET /api/v1/products
# Response:
[
  { "id": 1, "name": "Laptop", "price": 999.99 },
  { "id": 2, "name": "Mouse", "price": 29.99 }
]
```

#### Version 2 (Enhanced Format)
```http
GET /api/v2/products
# Response:
{
  "data": [
    {
      "id": 1,
      "name": "Laptop",
      "price": 999.99,
      "category": "Electronics",
      "inStock": true,
      "createdAt": "2024-07-24T10:30:00Z"
    }
  ],
  "total": 1,
  "version": "2.0"
}
```

### Testing with curl

```bash
# Version 1
curl -X GET "http://localhost:5163/api/v1/product" -H "accept: application/json"

# Version 2
curl -X GET "http://localhost:5163/api/v2/product" -H "accept: application/json"

# Create product (v2 only)
curl -X POST "http://localhost:5163/api/v2/product" \
  -H "Content-Type: application/json" \
  -d '{"name":"Gaming Mouse","price":89.99,"category":"Gaming"}'
```

## 🔍 Key Differences Between Versions

| Feature | Version 1.0 | Version 2.0 |
|---------|-------------|-------------|
| **Data Format** | Simple objects | Enhanced with metadata |
| **Product Fields** | `Id`, `Name`, `Price` | + `Category`, `InStock`, `CreatedAt`, `Description`, `Tags` |
| **Response Structure** | Direct array | Wrapped with metadata (`Data`, `Total`, `Version`) |
| **Available Operations** | GET only | GET + POST (Create) |
| **Validation** | Basic | Enhanced with detailed error messages |

### Version Evolution Strategy

- **v1.0**: Minimal viable product with core functionality
- **v2.0**: Enhanced features, richer data model, additional operations
- **Future versions**: Can add new fields, operations, or modify business logic

## 📖 Best Practices

### 1. **Consistent Versioning Strategy**
- Choose one versioning method and stick with it
- Use semantic versioning (Major.Minor format)
- Document version differences clearly

### 2. **Backward Compatibility**
- Keep older versions functional during transition periods
- Provide migration guides for breaking changes
- Use deprecation notices before removing versions

### 3. **Default Version Handling**
```csharp
options.DefaultApiVersion = new ApiVersion(1, 0);
options.AssumeDefaultVersionWhenUnspecified = true;
```

### 4. **Clear Documentation**
- Use `[EndpointSummary]` and `[EndpointDescription]` attributes
- Group endpoints by version using `[Tags]`
- Provide examples for each version

### 5. **Testing Strategy**
- Test all supported versions
- Validate version-specific behavior
- Use automated tests for regression prevention

### 6. **Monitoring and Analytics**
- Track version usage to understand adoption
- Monitor performance across versions
- Plan deprecation based on usage data

## 🔧 Configuration Options

The `Asp.Versioning` library supports multiple versioning strategies:

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    
    // Multiple readers can be combined
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),           // /api/v1/products
        new QueryStringApiVersionReader(),          // ?version=1.0
        new HeaderApiVersionReader("X-Version"),    // X-Version: 1.0
        new MediaTypeApiVersionReader()             // application/json;v=1.0
    );
});
```

## 📚 Additional Resources

- [ASP.NET Core API Versioning Documentation](https://github.com/dotnet/aspnet-api-versioning)
- [API Versioning Best Practices](https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design#versioning-a-restful-web-api)
- [OpenAPI Specification](https://swagger.io/specification/)
- [REST API Design Guidelines](https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
