using Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library
{
    public class UserService
    {
        private readonly BookstoreDbContext _context;

        private List<Book> _books;

        public UserService(BookstoreDbContext context)
        {
            _context = context;
            _books = _context.Books.Include(b => b.Author).Include(b => b.Genre).ToList();
        }

        public void RegisterUser(string username, string password)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                Console.WriteLine("A user with this username already exists.");
                return;
            }

            var user = new User
            {
                Username = username,
                Password = password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            Console.WriteLine("Registration successful!");
        }

        public User LoginUser(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                Console.WriteLine("Incorrect username or password.");
                return null;
            }

            Console.WriteLine("Login successful!");
            return user;
        }

        public void AddBook(Book book)
        {
            var author = _context.Authors.Find(book.AuthorId);
            var publisher = _context.Publishers.Find(book.PublisherId);
            var genre = _context.Genres.Find(book.GenreId);

            if (author == null || publisher == null || genre == null)
            {
                Console.WriteLine("Author, publisher, or genre not found. Please check your input.");
                return;
            }

            _context.Books.Add(book);
            _context.SaveChanges();
            Console.WriteLine("Book added successfully!");
        }
        public void DeleteBook(int bookId)
        {
            var book = _context.Books.Find(bookId);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
                Console.WriteLine("Book deleted successfully!");
            }
            else
            {
                Console.WriteLine("Book not found.");
            }
        }
        public void EditBook(int bookId, string newTitle, int newAuthorId, int newPublisherId, int newGenreId)
        {
            var book = _context.Books.Find(bookId);
            if (book != null)
            {
                book.Title = newTitle;
                book.AuthorId = newAuthorId;
                book.PublisherId = newPublisherId;
                book.GenreId = newGenreId;

                _context.SaveChanges();
                Console.WriteLine("Book details updated successfully!");
            }
            else
            {
                Console.WriteLine("Book not found.");
            }
        }
        public void SellBook(int bookId)
        {
            var book = _context.Books.Find(bookId);
            if (book != null)
            {
                var sale = new Sale
                {
                    BookId = bookId,
                    SaleDate = DateTime.Now
                };

                _context.Sales.Add(sale);
                _context.SaveChanges();
                Console.WriteLine("Book sold successfully!");
            }
            else
            {
                Console.WriteLine("Book not found.");
            }
        }
        public void MarkBookOutOfStock(int bookId)
        {
            var book = _context.Books.Find(bookId);
            if (book != null)
            {
                book.IsOutOfStock = true;
                _context.Books.Update(book);
                _context.SaveChanges();
                Console.WriteLine("Book marked as out of stock successfully!");
            }
            else
            {
                Console.WriteLine("Book not found.");
            }
        }

        public void PutBookOnDiscount(int bookId, double discountPercentage)
        {
            var book = _context.Books.Find(bookId);
            if (book != null)
            {
                if (discountPercentage < 0 || discountPercentage > 100)
                {
                    Console.WriteLine("Discount percentage must be between 0 and 100.");
                    return;
                }

                book.DiscountPercentage = discountPercentage;
                _context.Books.Update(book);
                _context.SaveChanges();
                Console.WriteLine($"Book now has a {discountPercentage}% discount!");
            }
            else
            {
                Console.WriteLine("Book not found.");
            }
        }

        public void ReserveBookForCustomer(int bookId, string customerUsername)
        {
            var book = _context.Books.Find(bookId);
            var user = _context.Users.SingleOrDefault(u => u.Username == customerUsername);
            if (book != null && user != null)
            {
                if (book.IsOutOfStock)
                {
                    Console.WriteLine("Book is not available for reservation.");
                    return;
                }

                var reservation = new Reservation
                {
                    BookId = bookId,
                    UserId = user.Id,
                    ReservationDate = DateTime.Now
                };
                book.ReservedFor = user.Id; 
                _context.Books.Update(book);
                _context.Reservations.Add(reservation);
                _context.SaveChanges();

                Console.WriteLine("Book reserved successfully for the user!");
            }
            else
            {
                if (book == null) Console.WriteLine("Book not found.");
                if (user == null) Console.WriteLine("User not found.");
            }
        }

        public void SearchBooksByTitle(string title)
        {
            var results = _books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
            DisplaySearchResults(results);
        }

        public void SearchBooksByAuthor(string authorName)
        {
            var results = _books.Where(b => b.Author.FullName.Contains(authorName, StringComparison.OrdinalIgnoreCase)).ToList();
            DisplaySearchResults(results);
        }

        public void SearchBooksByGenre(string genreName)
        {
            var results = _books.Where(b => b.Genre.Name.Contains(genreName, StringComparison.OrdinalIgnoreCase)).ToList();
            DisplaySearchResults(results);
        }

        public void ViewNewReleases()
        {
            var newReleases = _books.Where(b => b.ReleaseDate >= DateTime.Now.AddMonths(-1)).ToList();
            DisplaySearchResults(newReleases);
        }

        public void ViewMostPopularBooks()
        {
            var popularBooks = _books.OrderByDescending(b => b.Popularity).Take(10).ToList();
            DisplaySearchResults(popularBooks);
        }

        public void ViewMostPopularAuthors()
        {
            var popularAuthors = _books.GroupBy(b => b.Author.FullName)
                                       .OrderByDescending(g => g.Count())
                                       .Take(10)
                                       .Select(g => g.Key)
                                       .ToList();
            Console.WriteLine("Most Popular Authors:");
            foreach (var author in popularAuthors)
            {
                Console.WriteLine(author);
            }
        }

        public void ViewMostPopularGenres()
        {
            var popularGenres = _books.GroupBy(b => b.Genre.Name)
                                      .OrderByDescending(g => g.Count())
                                      .Take(10)
                                      .Select(g => g.Key)
                                      .ToList();
            Console.WriteLine("Most Popular Genres:");
            foreach (var genre in popularGenres)
            {
                Console.WriteLine(genre);
            }
        }

        private void DisplaySearchResults(List<Book> results)
        {
            if (results.Count == 0)
            {
                Console.WriteLine("No results found.");
                return;
            }

            foreach (var book in results)
            {
                Console.WriteLine($"Title: {book.Title}, Author: {book.Author.FullName}, Genre: {book.Genre.Name}, Popularity: {book.Popularity}");
            }
        }
    }
}