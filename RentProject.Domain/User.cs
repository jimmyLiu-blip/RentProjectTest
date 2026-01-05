namespace RentProject.Domain
{
    public class Users
    {
        public int UserId { get; set; } 

        public string UserName { get; set; } = null!;

        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? DeletedAt { get; set; }
    }
}
