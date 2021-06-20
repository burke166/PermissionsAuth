using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PermissionsAuth.Data
{
    public class UserRole
    {
        public UserRole()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string Name { get; set; }
        public ICollection<UserAccountUserRole> UserAccounts { get; set; }
        internal string _Permissions { get; set; }

        [NotMapped]
        public List<string> Permissions
        {
            get
            {
                return string.IsNullOrWhiteSpace(_Permissions) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(_Permissions);
            }
            private set
            {
                _Permissions = JsonSerializer.Serialize(value);
            }
        }

        public void AddPermission(string permission)
        {
            var permissions = this.Permissions;
            permissions.Add(permission);
            Permissions = permissions;
        }

        public void DeletePermission(string permission)
        {
            var permissions = this.Permissions;
            permissions.Remove(permission);
            Permissions = permissions;
        }

        public void AddPermissions(ICollection<string> permissions)
        {
            var _permissions = this.Permissions;
            _permissions.AddRange(permissions);
            Permissions = _permissions;
        }

        public void DeletePermissions(ICollection<string> permissions)
        {
            var _permissions = this.Permissions;
            foreach (string permission in permissions)
                _permissions.Remove(permission);
            Permissions = _permissions;
        }
    }
}
