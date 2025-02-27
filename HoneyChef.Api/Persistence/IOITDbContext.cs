using HoneyChef.Api.Entities;
using IOITCore.Entities;
using IOITCore.Entities.Bases;
using IOITCore.Persistence.Base;
using Microsoft.AspNetCore.Mvc.Infrastructure;



//using IOITCore.Interfaces.Helpers;
using Microsoft.EntityFrameworkCore;

namespace IOITCore.Persistence
{
    public class IOITDbContext : DbContext
    {
        //private readonly IDateTimeService _dateTime;
        public IOITDbContext(DbContextOptions<IOITDbContext> options) : base(options)
        {
            //_dateTime = datetime;
        }

        
        public virtual DbSet<Function> Functions { get; set; }
        public virtual DbSet<FunctionRole> FunctionRoles { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserMapping> UserMappings { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<LogAction> LogActions { get; set; }
        // main db manager
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Country> Countrys { get; set; }
        public virtual DbSet<DetailDirection> DetailDirections { get; set; }
        public virtual DbSet<Direction> Directions { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeCategory> recipeCategories { get; set; }
        //
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var assembly = typeof(EntityTypeBaseConfiguration<>).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);


            modelBuilder.Entity<Function>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Url).HasMaxLength(200);
            });

            modelBuilder.Entity<FunctionRole>(entity =>
            {
                entity.Property(e => e.ActiveKey).HasMaxLength(20);

            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.KeyLock).HasMaxLength(8);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RegEmail).HasMaxLength(50);

                entity.Property(e => e.LastLoginAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserMapping>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<LogAction>(entity =>
            {
                entity.Property(e => e.ActionName).HasMaxLength(100);
                entity.Property(e => e.IpAddress).HasMaxLength(100);
            });

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddAuditUserChange();

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            AddAuditUserChange();

            return base.SaveChanges();
        }

        private void AddAuditUserChange()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity<int>>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.Now;
                        entry.Entity.UpdatedAt = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.Now;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<BaseEntity<Guid>>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.Now;
                        entry.Entity.UpdatedAt = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.Now;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<BaseEntity<long>>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.Now;
                        entry.Entity.UpdatedAt = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.Now;
                        break;
                }
            }
        }
    }
}
