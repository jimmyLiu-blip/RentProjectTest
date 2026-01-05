namespace RentProject.Domain.Junction
{
    // 案件和測試樣品關聯表
    public class ProjectSample
    {
        public int ProjectSampleId { get; set; }

        public int ProjectId { get; set; }

        public int SampleId { get; set; }
    }
}
