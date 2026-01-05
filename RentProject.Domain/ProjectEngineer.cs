namespace RentProject.Domain
{
    public class ProjectEngineer
    {
        public int ProjectEngineerId { get; set; }

        public string ProjectEngineerName { get; set; } = null!;

        public DateTime? DeletedAt { get; set; }
    }
}
