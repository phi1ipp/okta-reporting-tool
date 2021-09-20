using System.Collections.Generic;
using System.Linq;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// Class to define Okta user attributes and operations on them
    /// </summary>
    public static class UserAttributes
    {
        /// <summary>
        /// Collection of non-profile attributes available for reporting and operations
        /// </summary>
        public static ICollection<string> NonProfileAttributes => new List<string>
            {"status", "id", "created", "passwordChanged", "lastLogin", "provider", "transitioningToStatus"};

        /// <summary>
        /// Collection of group attributes which user is a member of
        /// </summary>
        public static ICollection<string> GroupAttributes => new List<string> {"grp.Name"};
    }

    /// <summary>
    /// Class to extend Okta User object with custom methods
    /// </summary>
    public static class OktaUserExtension
    {
        /// <summary>
        /// Method to get access to non-profile fields of User object by passing in their names
        /// </summary>
        /// <param name="user"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static string GetNonProfileAttribute(this IUser user, string attrName)
        {
            return attrName switch
            {
                "status" => user.Status,
                "transitioningToStatus" => user.TransitioningToStatus,
                "id" => user.Id,
                "created" => $"{user.Created:yyyy/MM/dd}",
                "passwordChanged" => $"{user.PasswordChanged:yyyy/MM/dd}",
                "lastLogin" => $"{user.LastLogin:yyyy/MM/dd}",
                "provider" => user.Credentials.Provider.Type.Value,
                _ => ""
            };
        }
    }
}