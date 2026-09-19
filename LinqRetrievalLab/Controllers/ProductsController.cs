using LinqRetrievalLab.Models.DATA;
using LinqRetrievalLab.Models.Domain;
using LinqRetrievalLab.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinqRetrievalLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            var products = await _context.Product
                .Include(p => p.Category)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Stock,
                    p.CategoryId,
                    Category = new
                    {
                        p.Category.Id,
                        p.Category.Name
                    }
                })
                .ToListAsync();

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductDTO dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Product data is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Product name is required.");
            }

            if (dto.Price < 0)
            {
                return BadRequest("Price cannot be negative.");
            }

            if (dto.Stock < 0)
            {
                return BadRequest("Stock cannot be negative.");
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId);

            if (category == null)
            {
                return BadRequest("Category does not exist.");
            }

            var product = new Product
            {
                Name = dto.Name.Trim(),
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId
            };

            _context.Product.Add(product);

            await _context.SaveChangesAsync();

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductWithCategory(int id)
    {
        try
        {
            var product = await _context.Product
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Stock,
                    p.CategoryId,
                    Category = new
                    {
                        p.Category.Id,
                        p.Category.Name
                    }
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> EditProduct(
        int id,
        ProductDTO dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Product data is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Product name is required.");
            }

            if (dto.Price < 0)
            {
                return BadRequest("Price cannot be negative.");
            }

            if (dto.Stock < 0)
            {
                return BadRequest("Stock cannot be negative.");
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId);

            if (category == null)
            {
                return BadRequest("Category does not exist.");
            }

            product.Name = dto.Name.Trim();
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _context.Product
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            _context.Product.Remove(product);

            await _context.SaveChangesAsync();

            return Ok("Product deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategory(int categoryId)
    {
        try
        {
            var category = await _context.Category
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            var products = await _context.Product
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Stock,
                    p.CategoryId,
                    Category = new
                    {
                        p.Category.Id,
                        p.Category.Name
                    }
                })
                .ToListAsync();

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search name is required.");
            }

            var products = await _context.Product
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(name))
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Stock,
                    p.CategoryId,
                    Category = new
                    {
                        p.Category.Id,
                        p.Category.Name
                    }
                })
                .ToListAsync();

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    [HttpGet("price-range")]
    public async Task<IActionResult> GetProductsByPriceRange(
        decimal lower,
        decimal upper)
    {
        try
        {
            if (lower < 0 || upper < 0)
            {
                return BadRequest("Price cannot be negative.");
            }

            if (lower > upper)
            {
                return BadRequest(
                    "Lower limit cannot be greater than upper limit.");
            }

            var products = await _context.Product
                .Include(p => p.Category)
                .Where(p => p.Price >= lower && p.Price <= upper)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Stock,
                    p.CategoryId,
                    Category = new
                    {
                        p.Category.Id,
                        p.Category.Name
                    }
                })
                .ToListAsync();

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }



    [HttpGet("stock-sum")]
    public async Task<IActionResult> GetStockSum()
    {
        try
        {
            var totalStock = await _context.Product
                .SumAsync(p => p.Stock);

            return Ok(new
            {
                TotalStock = totalStock
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }



    [HttpGet("average-price")]
    public async Task<IActionResult> GetAveragePrice()
    {
        try
        {
            var productCount = await _context.Product.CountAsync();

            if (productCount == 0)
            {
                return Ok(new
                {
                    AveragePrice = 0
                });
            }

            var averagePrice = await _context.Product
                .AverageAsync(p => p.Price);

            return Ok(new
            {
                AveragePrice = averagePrice
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    [HttpGet("count")]
    public async Task<IActionResult> GetProductCount()
    {
        try
        {
            var totalProducts = await _context.Product
                .CountAsync();

            return Ok(new
            {
                TotalProducts = totalProducts
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}