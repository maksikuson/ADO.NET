namespace Library.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
        public int PageCount { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public int PublicationYear { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public float Popularity { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool IsOutOfStock { get; set; }
        public double DiscountPercentage { get; set; }
        public int? ReservedFor { get; set; }
    }
}