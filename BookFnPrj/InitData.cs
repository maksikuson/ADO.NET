using Library.Entities;
using System;
using System.Linq;

namespace Library
{
    public class InitData
    {
        public static void SeedData(BookstoreDbContext context)
        {
            if (!context.Authors.Any())
            {
                var authors = new[]
                {
                    new Author { FullName = "Author One", Popularity = 1 },
                    new Author { FullName = "Author Two", Popularity = 2 }
                };
                context.Authors.AddRange(authors);
                context.SaveChanges();
            }

            if (!context.Publishers.Any())
            {
                var publishers = new[]
                {
                    new Publisher { Name = "Publisher One" },
                    new Publisher { Name = "Publisher Two" }
                };
                context.Publishers.AddRange(publishers);
                context.SaveChanges();
            }

            if (!context.Genres.Any())
            {
                var genres = new[]
                {
                    new Genre { Name = "Fiction" },
                    new Genre { Name = "Non-Fiction" },
                    new Genre { Name = "Science" }
                };
                context.Genres.AddRange(genres);
                context.SaveChanges();
            }

            if (!context.Books.Any())
            {
                var authors = context.Authors.ToList();
                var publishers = context.Publishers.ToList();
                var genres = context.Genres.ToList();

                var books = new[]
                {
                   new Book
{
    Title = "Mystery of the Lost Island",
    AuthorId = authors[2].Id,
    PublisherId = publishers[2].Id,
    GenreId = genres[0].Id,
    PageCount = 400,
    PublicationYear = 2023,
    CostPrice = 25.00m,
    SalePrice = 20.00m,
    Popularity = 8,
    ReleaseDate = new DateTime(2023, 3, 10),
    IsOutOfStock = false,
    DiscountPercentage = 15.0
},
new Book
{
    Title = "The Last Adventure",
    AuthorId = authors[3].Id,
    PublisherId = publishers[3].Id,
    GenreId = genres[1].Id,
    PageCount = 150,
    PublicationYear = 2022,
    CostPrice = 12.00m,
    SalePrice = 10.00m,
    Popularity = 6,
    ReleaseDate = new DateTime(2022, 11, 5),
    IsOutOfStock = true,
    DiscountPercentage = 5.0
},
new Book
{
    Title = "Cooking with Herbs",
    AuthorId = authors[4].Id,
    PublisherId = publishers[4].Id,
    GenreId = genres[2].Id,
    PageCount = 200,
    PublicationYear = 2021,
    CostPrice = 18.00m,
    SalePrice = 15.00m,
    Popularity = 7,
    ReleaseDate = new DateTime(2021, 7, 20),
    IsOutOfStock = false,
    DiscountPercentage = 20.0
},
new Book
{
    Title = "Tech Innovations 2024",
    AuthorId = authors[5].Id,
    PublisherId = publishers[5].Id,
    GenreId = genres[3].Id,
    PageCount = 350,
    PublicationYear = 2024,
    CostPrice = 30.00m,
    SalePrice = 28.00m,
    Popularity = 9,
    ReleaseDate = new DateTime(2024, 1, 15),
    IsOutOfStock = false,
    DiscountPercentage = 10.0
},
new Book
{
    Title = "A Journey Through Time",
    AuthorId = authors[6].Id,
    PublisherId = publishers[6].Id,
    GenreId = genres[4].Id,
    PageCount = 280,
    PublicationYear = 2020,
    CostPrice = 22.00m,
    SalePrice = 18.00m,
    Popularity = 4,
    ReleaseDate = new DateTime(2020, 5, 30),
    IsOutOfStock = true,
    DiscountPercentage = 0.0
}
                };
                context.Books.AddRange(books);
                context.SaveChanges();
            }
        }
    }
}