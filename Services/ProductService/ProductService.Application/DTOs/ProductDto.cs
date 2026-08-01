using ProductService.Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public Money Price { get; init; } = null!;
        public int StockQuantity { get; init; }
        public bool IsActive { get; init; }
        public Guid CategoryId { get; init; }
    }
}
