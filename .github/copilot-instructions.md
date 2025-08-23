# Copilot Instructions for API Versioning Playground

## Project Overview

This is a .NET 9 demonstration project showcasing API versioning strategies using the `Asp.Versioning` library. The solution contains two parallel implementations:
- **controllerBasedAPI**: Traditional controller-based approach with `Asp.Versioning.Mvc`
- **minimalAPI**: Modern minimal API approach with `Asp.Versioning.Http`

Both projects implement identical Product APIs with v1.0 (simple) and v2.0 (enhanced) versions.

## Architecture Patterns

### Versioning Strategy
- **URL Segment Versioning**: `/api/v{version}/products` (e.g., `/api/v1/products`, `/api/v2/products`)
- **Default Version**: 1.0 when unspecified
- **Version Reader**: `UrlSegmentApiVersionReader()` only

### Data Evolution Pattern
- **v1.0**: Simple objects `{ Id, Name, Price }`
- **v2.0**: Enhanced objects with metadata wrapper `{ Data: [...], Total, Version }` and additional fields `{ Category, InStock, CreatedAt, Description, Tags }`
- **v2.0 Only**: POST operations for creating products

### Controller-Based Implementation (`controllerBasedAPI/`)
```csharp
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")] / [ApiVersion("2.0")]
```
- Use `[ApiVersion]` attributes on action methods
- Separate methods for each version (e.g., `GetProductsV1()`, `GetProductsV2()`)
- Rich metadata with `[EndpointSummary]`, `[EndpointDescription]`, `[Tags]`

### Minimal API Implementation (`minimalAPI/`)
```csharp
var versionSet = app.NewApiVersionSet("Products API")
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .Build();

app.MapGroup("/api/v{version:apiVersion}/products")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(1, 0));
```
- Use version sets with `NewApiVersionSet()`
- Group endpoints with `MapGroup()`
- Static methods as handlers outside the main program flow
- Fluent configuration with `WithSummary()`, `WithDescription()`, `WithTags()`

## Development Workflows

### Running the Projects
```powershell
# Controller-based (port 5163)
cd controllerBasedAPI
dotnet run

# Minimal API (port 5050)  
cd minimalAPI
dotnet run
```

### Testing
- Both projects include `.http` files for REST Client testing
- OpenAPI documentation available at `/openapi/v1.json`
- Use version-specific endpoints for testing: `/api/v1/product` vs `/api/v2/product`

### Key Dependencies
- **controllerBasedAPI**: `Asp.Versioning.Mvc` (for controllers)
- **minimalAPI**: `Asp.Versioning.Http` (for minimal APIs)
- **Both**: `Microsoft.AspNetCore.OpenApi` for documentation

## Project-Specific Conventions

### Version-Specific Responses
- **v1**: Direct array response
- **v2**: Wrapped response with `{ Data, Total, Version }` metadata

### Error Handling
- Simple `BadRequest()` with string messages
- No complex validation or error models

### Data Models
- Anonymous objects for responses (no formal DTOs)
- `CreateProductRequest` record for POST operations (v2 only)
- Shared record definition in both projects

### Endpoint Naming
- Controller routes use `[controller]` token (maps to "Product")
- Minimal API uses explicit `/products` in route groups
- Both support `/{id:int}` parameter routes

## Integration Points

### OpenAPI Integration
- Automatic documentation generation for all versions
- Version-aware Swagger UI
- Rich metadata from attributes/fluent configuration

### Cross-Version Compatibility
- No shared models between versions
- Independent data structures allow breaking changes
- Version-specific validation rules

When working on this codebase, always maintain parallel implementations between controller-based and minimal API approaches, ensure version-specific data formats are preserved, and follow the established URL segment versioning pattern.
