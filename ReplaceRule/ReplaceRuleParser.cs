using Contract;
using System;

namespace ReplaceRule
{
    public class ReplaceRuleParser : IRenameRuleParser
    {
        public string Name => "Replace";

        public string Title => "Replace";

        public bool IsPlugAndPlay => false;

        public IRenameRule Parse(string line)
        {
            string[] tokens = line.Split(new string[] { "Replace " }, StringSplitOptions.None);
            string[] parts = tokens[1].Split(new string[] { " => " }, StringSplitOptions.None);

            string needle = parts[0].Replace("\"", "");
            string replacer = parts[1].Replace("\"", "");

            IRenameRule rule = new ReplaceRule(needle, replacer);

            return rule;
        }
    }
}
