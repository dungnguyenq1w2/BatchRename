using Contract;
using System.IO;

namespace AddSuffixRule
{
    public class AddSuffixRule : IRenameRule
    {
        public string Suffix;

        public AddSuffixRule(string suffix)
        {
            Suffix = suffix;
        }

        public string Rename(string original)
        {
            string newName = Path.GetFileNameWithoutExtension(original) + Suffix + Path.GetExtension(original);

            return newName;
        }
    }
}
