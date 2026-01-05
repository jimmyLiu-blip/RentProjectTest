namespace RentProject.Domain
{
    public class Project
    {
        public int ProjectId { get; set; }

        public string ProjectNo { get; set; } = null!;

        public string ProjectName { get; set; } = null!;

        public int CustomerId { get; set; }

        public int SaleId { get; set; }

        public int ProjectEngineerId { get; set; }

        public int ProjectOwnerId { get; set; }

        public DateTime ConfirmDate { get; set; }

        public string JobNo { get; set; } = null!;

        public decimal Amount {get; set;}

        public DateTime? DeletedAt {  get; set; }

    }
}
