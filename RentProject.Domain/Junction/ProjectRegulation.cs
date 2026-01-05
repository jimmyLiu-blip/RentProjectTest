namespace RentProject.Domain.Junction
{
    public class ProjectRegulation
    {
        public int ProjectRegulationId { get; set; }

        public int ProjectId { get; set; }

        public int RegulationId { get; set; }

        // 是否使用該法規的所有預設測試標準
        public bool UseDefaultTestStandards { get; set; } = true;
    }
}
