using Contract;
using System;

namespace AddSuffixRule
{
    public class AddSuffixRuleParser : IRenameRuleParser
    {
        public string Name => "AddSuffix";
        public string Title => "Add suffix";
        public bool IsPlugAndPlay => false;

        public IRenameRule Parse(string line)
        {
            string[] tokens = line.Split(new string[] { "AddSuffix " }, StringSplitOptions.None);

            string suffix = tokens[1].Replace("\"", "");
            IRenameRule rule = new AddSuffixRule(suffix);

            return rule;
        }
    }
}
