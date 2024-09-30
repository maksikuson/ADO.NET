using Library.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Library
{
    public class BookstoreDbContextFactory : IDesignTimeDbContextFactory<BookstoreDbContext>
    {
        public BookstoreDbContext CreateDbContext(string[] args = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookstoreDbContext>();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new BookstoreDbContext(optionsBuilder.Options);
        }
    }

    public class BookstoreDbContext : DbContext
    {
        public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasKey(b => b.Id);
            modelBuilder.Entity<Book>()
                .Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<Book>()
                .Property(b => b.Popularity)
                .IsRequired()
                .HasDefaultValue(0);
            modelBuilder.Entity<Book>()
                .Property(b => b.IsOutOfStock)
                .IsRequired()
                .HasDefaultValue(false);
            modelBuilder.Entity<Book>()
                .Property(b => b.DiscountPercentage)
                .IsRequired()
                .HasDefaultValue(0.0);
            modelBuilder.Entity<Book>()
                .Property(b => b.ReservedFor)
                .IsRequired(false);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId);
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId);

            // Author
            modelBuilder.Entity<Author>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Author>()
                .Property(a => a.FullName)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Author>()
                .Property(a => a.Popularity)
                .IsRequired()
                .HasDefaultValue(0);

            // Publisher
            modelBuilder.Entity<Publisher>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Publisher>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Genre
            modelBuilder.Entity<Genre>()
                .HasKey(g => g.Id);
            modelBuilder.Entity<Genre>()
                .Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(50);

            // User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(100);

            // Sale
            modelBuilder.Entity<Sale>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<Sale>()
                .Property(s => s.SaleDate)
                .IsRequired();
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Book)
                .WithMany()
                .HasForeignKey(s => s.BookId);

            // Reservation
            modelBuilder.Entity<Reservation>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Book)
                .WithMany()
                .HasForeignKey(r => r.BookId);
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddDbContext<BookstoreDbContext>(options =>
                options.UseSqlServer("Source=DESKTOP-9U0F2B0\\SQLEXPRESSS;Database=Library;Integrated Security=True;TrustServerCertificate=True"));
            services.AddScoped<UserService>();
            services.AddScoped<MenuManager>();

            var serviceProvider = services.BuildServiceProvider();

            services.AddScoped<MenuManager>(provider =>
            {
                var userService = provider.GetRequiredService<UserService>();
                var context = provider.GetRequiredService<BookstoreDbContext>();
                return new MenuManager(userService, context);
            });


            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<BookstoreDbContext>();
                context.Database.Migrate();
                InitData.SeedData(context);
            }

            using (var scope = serviceProvider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetService<UserService>();
                userService.RegisterUser("testUser", "password123");
            }

            using (var scope = serviceProvider.CreateScope())
            {
                var menuManager = scope.ServiceProvider.GetService<MenuManager>();
                menuManager.ShowMenu();
            }
        }
    }
}