using InventoryModels;
using InventoryModels.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace InventoryDatabaseCore
{
    public class InventoryDbContext : DbContext
    {
        private static IConfigurationRoot _configuration;

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryColor> CategoryColors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GetItemsForListingDto> ItemsForListing { get; set; }
        public DbSet<AllItemsPipeDelimitedStringDto> AllItemsOutput { get; set; }
        public DbSet<GetItemsTotalValueDto> GetItemsTotalValues { get; set; }
        public DbSet<ItemsWithGenresDto> ItemsWithGenres { get; set; }
        public InventoryDbContext() : base() { }

        public InventoryDbContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //unique, non-clustered index for ItemGenre relationships
            modelBuilder.Entity<ItemGenre>()
                        .HasIndex(ig => new { ig.ItemId, ig.GenreId })
                        .IsUnique()
                        .IsClustered(false);

            modelBuilder.Entity<GetItemsForListingDto>(x =>
            {
                x.HasNoKey();
                x.ToView("ItemsForListing");
            });

            modelBuilder.Entity<AllItemsPipeDelimitedStringDto>(x =>
            {
                x.HasNoKey();
                x.ToView("AllItemsOutput");
            });

            modelBuilder.Entity<GetItemsTotalValueDto>(x =>
            {
                x.HasNoKey();
                x.ToView("GetItemsTotalValues");
            });

            var createdDate = new DateTime(2020, 01, 01);
            modelBuilder.Entity<Genre>(x => {
                x.HasData(
                    new Genre() { Id = 1, CreatedDate = createdDate, IsActive = true
                                    , IsDeleted = false, Name = "Fantasy" },
                    new Genre() { Id = 2, CreatedDate = createdDate, IsActive = true
                                    , IsDeleted = false, Name = "Sci/Fi" },
                    new Genre() { Id = 3, CreatedDate = createdDate, IsActive = true
                                    , IsDeleted = false, Name = "Horror" },
                    new Genre() { Id = 4, CreatedDate = createdDate, IsActive = true
                                    , IsDeleted = false, Name = "Comedy" },
                    new Genre() { Id = 5, CreatedDate = createdDate, IsActive = true
                                    , IsDeleted = false, Name = "Drama" }
                );
            });


            modelBuilder.Entity<ItemsWithGenresDto>(x =>
            {
                x.HasNoKey();
                x.ToView("ItemsWithGenres");
            });

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                _configuration = builder.Build();
                var cnstr = _configuration.GetConnectionString("InventoryManager");
                optionsBuilder.UseSqlServer(cnstr);
            }
        }

        public override int SaveChanges()
        {
            var tracker = ChangeTracker;

            foreach (var entry in tracker.Entries())
            {
                if (entry.Entity is FullAuditModel)
                {
                    var referenceEntity = entry.Entity as FullAuditModel;
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            referenceEntity.CreatedDate = System.DateTime.Now;
                            break;
                        case EntityState.Deleted:
                        case EntityState.Modified:
                            referenceEntity.LastModifiedDate = System.DateTime.Now;
                            break;
                        default:
                            break;
                    }
                }
            }

            return base.SaveChanges();
        }
    }
}
