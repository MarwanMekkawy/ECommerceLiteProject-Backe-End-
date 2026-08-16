using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();

        private Category() { } 

        public Category(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required.");

            Name = name.Trim();
            Description = description?.Trim();
            IsActive = true;
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required.");

            Name = name.Trim();
        }

        public void ChangeDescription(string? description)
        {
            Description = description?.Trim();
        }

        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException($"[{Name}] Category is already active.");

            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException($"[{Name}] Category is already inactive.");

            IsActive = false;
        }
    }
}
