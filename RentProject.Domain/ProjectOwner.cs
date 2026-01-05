namespace RentProject.Domain
{
    public class ProjectOwner
    {
        public int ProjectOwnerId { get; set; }

        public string ProjectOwnerName { get; set; } = null!;

        public DateTime? DeletedAt { get; set; }
    }
}
