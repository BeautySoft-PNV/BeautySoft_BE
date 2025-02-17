namespace BeautySoftBE.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public int RoleId { get; set; }
        public RoleModel Role { get; set; }
    }
}