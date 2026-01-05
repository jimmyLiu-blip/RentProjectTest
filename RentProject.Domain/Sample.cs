namespace RentProject.Domain
{
    public class Sample
    {
        public int SampleId { get; set; }

        public string SampleModel { get; set; } = null!;

        public string SampleNo { get; set; } = null!;

        public int CustomerId { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
