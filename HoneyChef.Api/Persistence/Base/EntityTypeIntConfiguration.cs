using IOITCore.Entities.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static IOITCore.Enums.ApiEnums;

namespace IOITCore.Persistence.Base
{
    public class EntityTypeIntConfiguration<T> : IEntityTypeConfiguration<T> where T : AbstractEntity<int>
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(i => i.UpdatedAt).HasDefaultValueSql("getdate()");
            builder.Property(i => i.CreatedById).IsRequired(false);
            builder.Property(i => i.UpdatedById).IsRequired(false);
            builder.Property(i => i.Status).HasDefaultValue(EntityStatus.NORMAL);
            builder.Property(i => i.CompanyId).HasDefaultValue(0).IsRequired(true);
        }
    }
}
