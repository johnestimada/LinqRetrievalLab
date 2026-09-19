using LinqRetrievalLab.Models.DATA;
using LinqRetrievalLab.Models.Domain;
using LinqRetrievalLab.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinqRetrievalLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _context.Category
                .Select(c => new
                {
                    c.Id,
                    c.Name
                })
                .ToListAsync();

            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryDTO dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Category data is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Category name is required.");
            }

            var category = new Category
            {
                Name = dto.Name.Trim()
            };

            _context.Category.Add(category);

            await _context.SaveChangesAsync();

            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> EditCategory(
        int id,
        CategoryDTO dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Category data is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Category name is required.");
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            category.Name = dto.Name.Trim();

            await _context.SaveChangesAsync();

            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var category = await _context.Category
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            if (category.Products.Any())
            {
                return BadRequest(
                    "Cannot delete category because it has products.");
            }

            _context.Category.Remove(category);

            await _context.SaveChangesAsync();

            return Ok("Category deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}