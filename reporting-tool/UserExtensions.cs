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
            var attrList = attributes.ToList();

            var values = new[]
                {
                    new[] {$"{user.Id}"},
                    attrList
                        .Where(attr => UserAttributes.NonProfileAttributes.Contains(attr))
                        .Select(user.GetNonProfileAttribute),
                    attrList
                        .Where(attr =>
                            !UserAttributes.NonProfileAttributes.Contains(attr) &&
                            !UserAttributes.GroupAttributes.Contains(attr))
                        .Select(attr =>
                        {
                            if (user.Profile[attr] is IEnumerable<object> coll)
                            {
                                var str = string.Join(',', coll.Select(val => val.ToString()));
                                return $"({str})";
                            }

                            return user.Profile[attr]?.ToString();
                        })
                }
                .Where(lst => lst.Any())
                .SelectMany(x => x)
                .Select(attr => !string.IsNullOrEmpty(attr) && attr.Contains(ofs) ? $"\"{attr}\"" : attr);

            if (!attrList.Any(a => UserAttributes.GroupAttributes.Contains(a))) return string.Join(ofs, values);

            if (oktaClient == null)
            {
                throw new Exception("Can't get group attributes without an instance of Okta Client");
            }

            //todo Name hardcoded as the only supported attribute for now
            values = values.Concat(new[] { await oktaClient.Users
                .ListUserGroups(user.Id)
                .Select(grp =>
                    $"({grp.Profile.Name})")
                .Aggregate(string.Concat) });

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
            var attrList = attributes.ToList();

            return string.Join(ofs,
                new[]
                    {
                        new[] {"id"},
                        attrList.Where(attr => UserAttributes.NonProfileAttributes.Contains(attr)),
                        attrList.Where(attr =>
                            !UserAttributes.NonProfileAttributes.Contains(attr) &&
                            !UserAttributes.GroupAttributes.Contains(attr)),
                        attrList.Where(attr => UserAttributes.GroupAttributes.Contains(attr))
                    }.Where(en => en.Any())
                    .SelectMany(x => x));
        }
    }
}