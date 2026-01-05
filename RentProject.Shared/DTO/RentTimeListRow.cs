namespace RentProject.Shared.DTO
{
    // 放清單需要顯示的欄位
    public class RentTimeListRow
    {
        public int RentTimeId { get; set; }

        public string BookingNo { get; set; } = null!;

        public string Area { get; set; } = "";

        public string Location { get; set; } = "";

        public string CustomerName { get; set; } = "";

        public string PE { get; set; } = "";

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string ProjectNo { get; set; } = "";

        public string ProjectName { get; set; } = "";
    }
}
