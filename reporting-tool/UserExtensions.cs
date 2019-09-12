using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// User extension methods
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Method to get a printable form of Okta User Attributes
        /// </summary>
        /// <param name="user">Okta User object</param>
        /// <param name="attributes">Collection of attribute names</param>
        /// <param name="oktaClient">Okta Client</param>
        /// <param name="ofs">Output fields separator</param>
        /// <returns>String representing user attributes</returns>
        public static async Task<string> PrintAttributesAsync(this IUser user, IEnumerable<string> attributes,
            IOktaClient oktaClient = null, string ofs = " ")
        {
            var enumerable = attributes.ToList();

            if (enumerable.Any(attr => UserAttributes.GroupAttributes.Contains(attr)) && oktaClient == null)
            {
                throw new Exception("Can't get group attributes without an instance of Okta Client");
            }

            var values = new[]
                {
                    new[] {$"{user.Id}"},
                    enumerable
                        .Where(attr => UserAttributes.NonProfileAttributes.Contains(attr))
                        .Select(user.GetNonProfileAttribute),
                    enumerable
                        .Where(attr =>
                            !UserAttributes.NonProfileAttributes.Contains(attr) &&
                            !UserAttributes.GroupAttributes.Contains(attr))
                        .Select(attr => user.Profile[attr]?.ToString()),
                }
                .Where(lst => lst.Any())
                .SelectMany(x => x)
                .Select(attr => !string.IsNullOrEmpty(attr) && attr.Contains(ofs) ? $"\"{attr}\"" : attr);

            if (enumerable.Any(a => UserAttributes.GroupAttributes.Contains(a)))
                values = values.Concat(new[]
                {
                    await oktaClient.Users
                        .ListUserGroups(user.Id)
                        .Select(grp => $"({grp.Profile.Name})")
                        .Aggregate(string.Concat)
                });

            return string.Join(ofs, values);
        }

        /// <summary>
        /// Method to get a printable form of user attributes header
        /// </summary>
        /// <param name="attributes">Collection of attribute names</param>
        /// <param name="ofs">Output fields separator</param>
        /// <returns>String representing header row</returns>
        public static string PrintUserAttributesHeader(IEnumerable<string> attributes, string ofs = " ")
        {
            var enumerable = attributes.ToList();

            return string.Join(ofs,
                new[]
                    {
                        new[] {"id"},
                        enumerable.Where(attr => UserAttributes.NonProfileAttributes.Contains(attr)),
                        enumerable.Where(attr =>
                            !UserAttributes.NonProfileAttributes.Contains(attr) &&
                            !UserAttributes.GroupAttributes.Contains(attr)),
                        enumerable.Where(attr => UserAttributes.GroupAttributes.Contains(attr))
                    }.Where(en => en.Any())
                    .SelectMany(x => x));
        }
    }
}