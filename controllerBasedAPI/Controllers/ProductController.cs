using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace controllerBasedAPI.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }

    // Version 1 endpoints
    [HttpGet]
    [ApiVersion("1.0")]
    [EndpointSummary("Get all products (v1)")]
    [EndpointDescription("Returns a simple list of products in version 1 format")]
    [Tags("Products v1")]
    public IActionResult GetProductsV1()
    {
        var products = new[]
        {
            new { Id = 1, Name = "Laptop", Price = 999.99m },
            new { Id = 2, Name = "Mouse", Price = 29.99m },
            new { Id = 3, Name = "Keyboard", Price = 79.99m }
        };

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ApiVersion("1.0")]
    [EndpointSummary("Get product by ID (v1)")]
    [EndpointDescription("Returns a single product by ID in version 1 format")]
    [Tags("Products v1")]
    public IActionResult GetProductByIdV1(int id)
    {
        if (id <= 0) 
            return BadRequest("Invalid product ID");

        var product = new { Id = id, Name = $"Product {id}", Price = 99.99m * id };
        return Ok(product);
    }

    // Version 2 endpoints
    [HttpGet]
    [ApiVersion("2.0")]
    [EndpointSummary("Get all products (v2)")]
    [EndpointDescription("Returns enhanced product list with additional fields in version 2 format")]
    [Tags("Products v2")]
    public IActionResult GetProductsV2()
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

        return Ok(new
        {
            Data = products,
            Total = products.Length,
            Version = "2.0"
        });
    }

    [HttpGet("{id:int}")]
    [ApiVersion("2.0")]
    [EndpointSummary("Get product by ID (v2)")]
    [EndpointDescription("Returns enhanced product details in version 2 format")]
    [Tags("Products v2")]
    public IActionResult GetProductByIdV2(int id)
    {
        if (id <= 0) 
            return BadRequest("Invalid product ID");

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

        return Ok(product);
    }

    [HttpPost]
    [ApiVersion("2.0")]
    [EndpointSummary("Create product (v2)")]
    [EndpointDescription("Creates a new product with enhanced validation (v2 only)")]
    [Tags("Products v2")]
    public IActionResult CreateProductV2([FromBody] CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Product name is required");

        if (request.Price <= 0)
            return BadRequest("Price must be greater than 0");

        var newProduct = new
        {
            Id = Random.Shared.Next(1000, 9999),
            request.Name,
            request.Price,
            request.Category,
            InStock = true,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(
            nameof(GetProductByIdV2),
            new { id = newProduct.Id, version = "2.0" },
            newProduct);
    }
}

public record CreateProductRequest(string Name, decimal Price, string Category);
