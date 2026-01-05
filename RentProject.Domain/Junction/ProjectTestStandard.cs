namespace RentProject.Domain.Junction
{
    public class ProjectTestStandard
    {
        public int ProjectTestStandardId { get; set; }

        public int ProjectId { get; set; }

        public int ProjectRegulationId { get; set; } // 必須關聯到某個 ProjectRegulation

        public int TestStandardId { get; set; }
    }
}
