using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllLowerCaseRule
{
    public class AllLowerCaseRuleParser : IRenameRuleParser
    {
        public string Name => "AllLowerCase";

        public string Title => "All lower case";

        public bool IsPlugAndPlay => true;

        public IRenameRule Parse(string line)
        {
            IRenameRule rule = new AllLowerCaseRule();

            return rule;
        }
    }
}
