using Blog.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.API.Controllers;

/// <summary>
/// Categories Controller - Kategori yönetimi
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IUnitOfWork unitOfWork, ILogger<CategoriesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Tüm kategorileri getir
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            
            var categoryDtos = categories.Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.Slug,
                PostCount = _unitOfWork.Posts.GetQueryable()
                    .Where(p => p.CategoryId == c.Id && p.Status == Domain.Entities.PostStatus.Published)
                    .Count()
            }).ToList();

            return Ok(categoryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kategoriler getirilirken hata oluştu");
            return StatusCode(500, new { Error = "Kategoriler getirilirken hata oluştu" });
        }
    }

    /// <summary>
    /// Kategori detayını getir
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            
            if (category == null)
            {
                return NotFound("Kategori bulunamadı");
            }

            var categoryDto = new
            {
                category.Id,
                category.Name,
                category.Description,
                category.Slug
            };

            return Ok(categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kategori getirilirken hata oluştu - CategoryId: {CategoryId}", id);
            return StatusCode(500, new { Error = "Kategori getirilirken hata oluştu" });
        }
    }

    /// <summary>
    /// Slug ile kategori getir
    /// </summary>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategoryBySlug(string slug)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetQueryable()
                .FirstOrDefaultAsync(c => c.Slug == slug);
            
            if (category == null)
            {
                return NotFound("Kategori bulunamadı");
            }

            var postCount = _unitOfWork.Posts.GetQueryable()
                .Where(p => p.CategoryId == category.Id && p.Status == Domain.Entities.PostStatus.Published)
                .Count();

            var categoryDto = new
            {
                category.Id,
                category.Name,
                category.Description,
                category.Slug,
                PostCount = postCount
            };

            return Ok(categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kategori getirilirken hata oluştu - Slug: {Slug}", slug);
            return StatusCode(500, new { Error = "Kategori getirilirken hata oluştu" });
        }
    }
} 