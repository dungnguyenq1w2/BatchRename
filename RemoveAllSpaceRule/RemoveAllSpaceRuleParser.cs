using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveAllSpaceRule
{
    public class RemoveAllSpaceRuleParser : IRenameRuleParser
    {
        public string Name => "RemoveAllSpace";

        public string Title => "Remove all space";

        public bool IsPlugAndPlay => true;

        public IRenameRule Parse(string line)
        {
            IRenameRule rule = new RemoveAllSpaceRule();

            return rule;
        }
    }
}
