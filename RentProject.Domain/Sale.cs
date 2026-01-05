namespace RentProject.Domain
{
    public class Sale
    {
        public int SaleId { get; set; }

        public string SaleName { get; set; } = null!;

        public DateTime? DeletedAt { get; set; }
    }
}
