using Contract;

namespace AddPrefixRule
{
    public class AddPrefixRule : IRenameRule
    {
        public string Prefix { get; }

        public AddPrefixRule(string prefix)
        {
            Prefix = prefix;
        }

        public string Rename(string original)
        {
            return $"{Prefix}{original}";
        }
    }
}
