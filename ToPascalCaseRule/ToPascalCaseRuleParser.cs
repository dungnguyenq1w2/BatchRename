using Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToPascalCaseRule
{
    public class ToPascalCaseRuleParser : IRenameRuleParser
    {
        public string Name => "ToPascalCase";

        public string Title => "To Pascal case";

        public bool IsPlugAndPlay => true;

        public IRenameRule Parse(string line)
        {
            IRenameRule rule = new ToPascalCaseRule();

            return rule;
        }
    }
}
