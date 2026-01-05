namespace RentProject.Domain
{
    public class CustomerContact
    {
        public int CustomerContactId { get; set; }

        public int CustomerId { get; set; }

        public string ContactName { get; set; } = null!;

        public string? ContactPhone { get; set; }

        public string? ContactEmail { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
