namespace RentProject.Domain
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = null!;

        public DateTime? DeletedAt { get; set; }
    }
}
