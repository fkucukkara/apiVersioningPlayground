using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader()
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

// Create API version set
var versionSet = app.NewApiVersionSet("Products API")
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .Build();

// Version 1 endpoints group
var v1Group = app.MapGroup("/api/v{version:apiVersion}/products")
    .WithTags("Products v1");

v1Group.MapGet("", GetProductsV1)
    .WithName("GetProductsV1")
    .WithSummary("Get all products (v1)")
    .WithDescription("Returns a simple list of products in version 1 format")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(1, 0));

v1Group.MapGet("/{id:int}", GetProductByIdV1)
    .WithName("GetProductByIdV1")
    .WithSummary("Get product by ID (v1)")
    .WithDescription("Returns a single product by ID in version 1 format")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(1, 0));

// Version 2 endpoints group
var v2Group = app.MapGroup("/api/v{version:apiVersion}/products")
    .WithTags("Products v2");

v2Group.MapGet("", GetProductsV2)
    .WithName("GetProductsV2")
    .WithSummary("Get all products (v2)")
    .WithDescription("Returns enhanced product list with additional fields in version 2 format")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(2, 0));

v2Group.MapGet("/{id:int}", GetProductByIdV2)
    .WithName("GetProductByIdV2")
    .WithSummary("Get product by ID (v2)")
    .WithDescription("Returns enhanced product details in version 2 format")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(2, 0));

v2Group.MapPost("", CreateProductV2)
    .WithName("CreateProductV2")
    .WithSummary("Create product (v2)")
    .WithDescription("Creates a new product with enhanced validation (v2 only)")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(2, 0));

app.Run();

// Version 1 handlers
static IResult GetProductsV1()
{
    var products = new[]
    {
        new { Id = 1, Name = "Laptop", Price = 999.99m },
        new { Id = 2, Name = "Mouse", Price = 29.99m },
        new { Id = 3, Name = "Keyboard", Price = 79.99m }
    };

    return TypedResults.Ok(products);
}

static IResult GetProductByIdV1(int id)
{
    if (id <= 0) return TypedResults.BadRequest("Invalid product ID");

    var product = new { Id = id, Name = $"Product {id}", Price = 99.99m * id };
    return TypedResults.Ok(product);
}

// Version 2 handlers
static IResult GetProductsV2()
{
    var products = new[]
    {
        new {
            Id = 1,
            Name = "Laptop",
            Price = 999.99m,
            Category = "Electronics",
            InStock = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        },
        new {
            Id = 2,
            Name = "Mouse",
            Price = 29.99m,
            Category = "Accessories",
            InStock = true,
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        },
        new {
            Id = 3,
            Name = "Keyboard",
            Price = 79.99m,
            Category = "Accessories",
            InStock = false,
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        }
    };

    return TypedResults.Ok(new
    {
        Data = products,
        Total = products.Length,
        Version = "2.0"
    });
}

static IResult GetProductByIdV2(int id)
{
    if (id <= 0) return TypedResults.BadRequest("Invalid product ID");

    var product = new
    {
        Id = id,
        Name = $"Enhanced Product {id}",
        Price = 99.99m * id,
        Category = "Sample Category",
        InStock = id % 2 == 0,
        CreatedAt = DateTime.UtcNow.AddDays(-id),
        Description = $"This is an enhanced description for product {id}",
        Tags = new[] { "electronics", "featured" }
    };

    return TypedResults.Ok(product);
}

static IResult CreateProductV2(CreateProductRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return TypedResults.BadRequest("Product name is required");

    if (request.Price <= 0)
        return TypedResults.BadRequest("Price must be greater than 0");

    var newProduct = new
    {
        Id = Random.Shared.Next(1000, 9999),
        request.Name,
        request.Price,
        request.Category,
        InStock = true,
        CreatedAt = DateTime.UtcNow
    };

    return TypedResults.Created($"/api/v2/products/{newProduct.Id}", newProduct);
}

public record CreateProductRequest(string Name, decimal Price, string Category);
