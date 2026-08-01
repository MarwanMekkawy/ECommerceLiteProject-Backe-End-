using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Queries.Categories
{
    public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository) : IQueryHandler<GetCategoryByIdQuery, CategoryDto>
    {
        public async Task<CategoryDto> HandleAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken = default)
        {
            var category = await categoryRepository.GetByIdUntrackedAsync(query.Id, cancellationToken);

            if (category is null)
                throw new NotFoundException("Category not found.");

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}
