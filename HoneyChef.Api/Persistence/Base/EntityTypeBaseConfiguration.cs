using IOITCore.Entities.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static IOITCore.Enums.ApiEnums;

namespace IOITCore.Persistence.Base
{
    public class EntityTypeBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity<int>
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.Status).HasDefaultValue(EntityStatus.NORMAL);
            builder.Property(i => i.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(i => i.UpdatedAt).HasDefaultValueSql("getdate()");
            builder.Property(i => i.CompanyId).HasDefaultValue(0).IsRequired(true);

        }
    }

    public class AppEntityTypeBaseLongConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity<long>
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.Status).HasDefaultValue(EntityStatus.NORMAL);
            builder.Property(i => i.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(i => i.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }

    public class AppEntityTypeBaseGuidConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity<Guid>
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.Status).HasDefaultValue(EntityStatus.NORMAL);
            builder.Property(i => i.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(i => i.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
