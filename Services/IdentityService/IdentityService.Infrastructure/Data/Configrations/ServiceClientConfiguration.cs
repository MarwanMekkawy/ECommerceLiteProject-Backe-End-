using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Data.Configrations
{
    public class ServiceClientConfiguration : IEntityTypeConfiguration<ServiceClient>
    {
        public void Configure(EntityTypeBuilder<ServiceClient> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClientId)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.ClientId)
                .IsUnique();

            builder.Property(x => x.ClientSecretHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.ServiceName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.IsActive)
                .IsRequired();
        }
    }
}
