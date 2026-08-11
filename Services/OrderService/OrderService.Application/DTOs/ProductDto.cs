using OrderService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public CurrencyCode Currency { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
    }
}
