namespace Library.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime ReservationDate { get; set; }
        public virtual Book Book { get; set; }
        public virtual User User { get; set; }
    }
}