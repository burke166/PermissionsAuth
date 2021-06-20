using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PermissionsAuth.Data
{
    public class UserAccount
    {
        public UserAccount()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string Auth0UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public UserAccountStatus Status { get; set; }

        public ICollection<UserAccountUserRole> UserRoles { get; set; }

    }
}
