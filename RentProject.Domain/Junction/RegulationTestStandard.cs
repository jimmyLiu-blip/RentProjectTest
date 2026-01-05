namespace RentProject.Domain.Junction
{
    // 法規和測試標準關聯表
    public class RegulationTestStandard
    {
        public int RegulationTestStandardId { get; set; }

        public int RegulationId { get; set; }

        public int TestStandardId { get; set; }
    }
}
