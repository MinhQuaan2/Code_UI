using CodeUI.Data.Entity;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.CategoryRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.CategoryResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Create a new category for elements
        /// </summary>
        /// <param name="request"> Basic category info</param>
        /// <returns></returns>
        [HttpPost("createCategory")]
        [Authorize(Roles = "SystemAdmin, Moderator")]
        public async Task<ActionResult<BaseResponseViewModel<QueryCategoryRequest>>> CreateCategory([FromBody] QueryCategoryRequest request)
        {
            try
            {
                var result = await _categoryService.CreateCategory(request);

                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get all categories available
        /// </summary>
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<QueryCategoryRequest>>> GetAllCategories([FromQuery] QueryCategoryRequest filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await _categoryService.GetCategories(filter, paging);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get single category by name or by ID
        /// </summary>
        /// <param name="id"> Get category only by ID </param>
        /// <param name="name"> Get category only by name</param>
        [HttpGet("getCategory")]
        public async Task<ActionResult<CategoryResponse>> GetSingleCategory([FromQuery] int? id = null, [FromQuery] string? name = null)
        {
            try
            {
                var result = await _categoryService.GetCategory(id, name);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Delete category
        /// </summary>
        [HttpDelete("deleteCategory")]
        public async Task<ActionResult<CategoryResponse>> DeleteCategory([FromQuery] string? name = null)
        {
            try
            {
                var result = await _categoryService.DeleteCategory(name);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
