namespace PermissionsAuth.Data
{
    public class UserAccountUserRole
    {
        public int UserAccountId { get; set; }
        public int UserRoleId { get; set; }
        public UserAccount UserAccount { get; set; }
        public UserRole UserRole { get; set; }
    }
}
