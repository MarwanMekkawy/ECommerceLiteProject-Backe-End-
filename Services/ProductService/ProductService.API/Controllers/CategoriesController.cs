using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Abstractions;
using ProductService.Application.Commands.Categories;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.Categories;

namespace ProductService.API.Controllers
{
    /// <summary>
    /// Provides endpoints for retrieving and managing categories, including
    /// category creation, updates, deletion, activation, and deactivation.
    /// Public retrieval endpoints return active categories, while administrator
    /// endpoints can retrieve active and inactive categories and manage categories.
    /// </summary>
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoriesController(
    IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>> getCategories,
    IQueryHandler<GetCategoryByIdQuery, CategoryDto?> getCategoryById,
    IQueryHandler<GetCategoryByNameQuery, CategoryDto?> getCategoryByName,
    IQueryHandler<SearchCategoriesByNameQuery, IReadOnlyList<CategoryDto>> searchCategories,
    ICommandHandler<CreateCategoryCommand, Guid> createCategory,
    ICommandHandler<UpdateCategoryCommand> updateCategory,
    ICommandHandler<DeleteCategoryCommand> deleteCategory,
    ICommandHandler<ActivateCategoryCommand> activateCategory,
    ICommandHandler<DeactivateCategoryCommand> deactivateCategory)
    : ControllerBase
    {
        /// <summary>
        /// Retrieves a paginated list of active categories.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Defaults to 1.</param>
        /// <param name="pageSize">The number of categories per page. Defaults to 10.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A paginated list of active categories.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetCategoriesQuery(pageNumber, pageSize);

            var result = await getCategories.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves an active category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The requested category.</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery(id);

            var result = await getCategoryById.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves an active category by its name.
        /// </summary>
        /// <param name="name">The name of the category to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The requested category.</returns>
        [HttpGet("by-name")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByNameQuery(name);

            var result = await getCategoryByName.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Searches for active categories whose names match the specified search term.
        /// </summary>
        /// <param name="name">The search term to use when searching category names.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A list of categories matching the search term.</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchCategories([FromQuery] string name, CancellationToken cancellationToken)
        {
            var query = new SearchCategoriesByNameQuery(name);

            var result = await searchCategories.HandleAsync(query, cancellationToken);

            return Ok(result);
        }


        // ========================= ADMIN ENDPOINTS =========================

        /// <summary>
        /// Retrieves a paginated list of all categories, including inactive categories.
        /// This endpoint is restricted to administrators.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Defaults to 1.</param>
        /// <param name="pageSize">The number of categories per page. Defaults to 10.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A paginated list of all categories.</returns>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCategoriesForAdmin([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetCategoriesQuery(pageNumber, pageSize, true);

            var result = await getCategories.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a category by its unique identifier, including inactive categories.
        /// This endpoint is restricted to administrators.
        /// </summary>
        /// <param name="id">The unique identifier of the category.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The requested category.</returns>
        [HttpGet("admin/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCategoryByIdForAdmin(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery(id, true);

            var result = await getCategoryById.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a category by its name, including inactive categories.
        /// This endpoint is restricted to administrators.
        /// </summary>
        /// <param name="name">The name of the category to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The requested category.</returns>
        [HttpGet("admin/by-name")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCategoryByNameForAdmin([FromQuery] string name, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByNameQuery(name, true);

            var result = await getCategoryByName.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Searches for categories whose names match the specified search term, including inactive categories.
        /// This endpoint is restricted to administrators.
        /// </summary>
        /// <param name="name">The search term to use when searching category names.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A list of categories matching the search term.</returns>
        [HttpGet("admin/search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchCategoriesForAdmin([FromQuery] string name, CancellationToken cancellationToken)
        {
            var query = new SearchCategoriesByNameQuery(name, true);

            var result = await searchCategories.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="dto">The information required to create the category.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The unique identifier of the newly created category.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto dto, CancellationToken cancellationToken = default)
        {
            var command = new CreateCategoryCommand(dto.Name, dto.Description);

            var result = await createCategory.HandleAsync(command, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The unique identifier of the category to update.</param>
        /// <param name="dto">The updated category information.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the category was updated successfully.</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequestDto dto, CancellationToken cancellationToken)
        {
            var command = new UpdateCategoryCommand(id, dto.Name, dto.Description);

            await updateCategory.HandleAsync(command, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Deletes an existing category.
        /// </summary>
        /// <param name="id">The unique identifier of the category to delete.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the category was deleted successfully.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand(id);

            await deleteCategory.HandleAsync(command, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Activates an existing category, making it available as an active category.
        /// </summary>
        /// <param name="id">The unique identifier of the category to activate.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the category was activated successfully.</returns>
        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateCategory(Guid id, CancellationToken cancellationToken)
        {
            var command = new ActivateCategoryCommand(id);

            await activateCategory.HandleAsync(command, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Deactivates an existing category, making it unavailable as an active category.
        /// </summary>
        /// <param name="id">The unique identifier of the category to deactivate.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the category was deactivated successfully.</returns>
        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateCategory(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeactivateCategoryCommand(id);

            await deactivateCategory.HandleAsync(command, cancellationToken);

            return Ok();
        }
    }
}
