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
    /// Handles category retrieval and management operations.
    /// Category retrieval endpoints are publicly accessible, while management operations require administrator authorization.
    /// </summary>
    [Route("api/V1/categories")]
    [ApiController]
    public class CategoriesController(
        IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>> getCategories,
        IQueryHandler<GetCategoryByIdQuery, CategoryDto?> getCategoryById,
        IQueryHandler<GetCategoryByNameQuery, CategoryDto?> getCategoryByName,
        IQueryHandler<SearchCategoriesByNameQuery, IReadOnlyList<CategoryDto>> searchCategories,
        ICommandHandler<CreateCategoryCommand, Guid> createCategory,
        ICommandHandler<UpdateCategoryCommand> updateCategory,
        ICommandHandler<DeleteCategoryCommand> deleteCategory)
        : ControllerBase

    {
        /// <summary>
        /// Retrieves a paginated list of categories.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Defaults to 1.</param>
        /// <param name="pageSize">The number of categories per page. Defaults to 10.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A paginated list of categories.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, CancellationToken cancellationToken = default)
        {
            var query = new GetCategoriesQuery(pageNumber, pageSize);

            var result = await getCategories.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a category by its unique identifier.
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
        /// Retrieves a category by its name.
        /// </summary>
        /// <param name="name">The name of the category to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The requested category.</returns>
        [HttpGet("by-name/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryByName(string name, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByNameQuery(name);

            var result = await getCategoryByName.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Searches for categories whose names match the specified search term.
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
    }
}
