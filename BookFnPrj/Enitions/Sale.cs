namespace Library.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalePrice { get; set; }
    }
}