using System.Collections.Generic;

namespace PermissionsAuth.Constants
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Create",
                $"Permissions.{module}.View",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete"
            };
        }

        public const string Any = "Permissions.Any";

        public const string All = "Permissions.All";

        public static string GetViewPermission(string module)
        {
            return $"Permissions.{module}.View";
        }

        public static string GetCreatePermission(string module)
        {
            return $"Permissions.{module}.Create";
        }

        public static string GetEditPermission(string module)
        {
            return $"Permissions.{module}.Edit";
        }

        public static string GetDeletePermission(string module)
        {
            return $"Permissions.{module}.Delete";
        }
    }
}
