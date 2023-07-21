using System;
using System.Linq;
using System.Threading.Tasks;

namespace reporting_tool
{
    /// <summary>
    /// Class to generate a report on all groups
    /// </summary>
    public class GroupList : OktaAction
    {
        private readonly string _ofs;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">OktaConfig instance</param>
        /// <param name="ofs">Output field separator</param>
        public GroupList(OktaConfig config, string ofs = " ") : base(config)
        {
            _ofs = ofs;
        }
        /// <summary>
        /// The report entry point
        /// </summary>
        public override async Task Run()
        {
            Console.WriteLine($"uuid{_ofs}type{_ofs}name");

            await OktaClient.Groups.ListGroups().ForEachAsync(grp => {
                var line = grp.Profile.Name.Contains(_ofs)
                    ? $"{grp.Id}{_ofs}{grp.Type}{_ofs}\"{grp.Profile.Name}\""
                    : $"{grp.Id}{_ofs}{grp.Type}{_ofs}{grp.Profile.Name}";

                Console.Out.WriteLine(line);
            });
            // await foreach (var grp in OktaClient.Groups.ListGroups())
            // {
            //     var line = grp.Profile.Name.Contains(_ofs)
            //         ? $"{grp.Id}{_ofs}{grp.Type}{_ofs}\"{grp.Profile.Name}\""
            //         : $"{grp.Id}{_ofs}{grp.Type}{_ofs}{grp.Profile.Name}";
                
            //     Console.Out.WriteLine(line);
            // }
        }
    }
}