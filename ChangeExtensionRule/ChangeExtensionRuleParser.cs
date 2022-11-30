using Contract;
using System;

namespace ChangeExtensionRule
{
    public class ChangeExtensionRuleParser : IRenameRuleParser
    {
        public string Name => "ChangeExtension";
        public string Title => "Change extension";
        public bool IsPlugAndPlay => false;

        public IRenameRule Parse(string line)
        {
            string[] tokens = line.Split(new string[] { "ChangeExtension " }, StringSplitOptions.None);

            string extension = tokens[1].Replace("\"", "");
            IRenameRule rule = new ChangeExtensionRule(extension);

            return rule;
        }
    }
}
