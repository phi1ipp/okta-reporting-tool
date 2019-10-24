using System;
using System.Linq;

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
        public override void Run()
        {
            Console.WriteLine($"uuid{_ofs}name");
            OktaClient.Groups.ListGroups().ForEach(grp =>
            {
                Console.WriteLine(grp.Profile.Name.Contains(_ofs)
                    ? $"{grp.Id}{_ofs}\"{grp.Profile.Name}\""
                    : $"{grp.Id}{_ofs}{grp.Profile.Name}");
            });
        }
    }
}