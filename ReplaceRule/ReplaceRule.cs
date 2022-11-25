using Contract;
using System.Collections.Generic;

namespace ReplaceRule
{
    public class ReplaceRule : IRenameRule
    {
        public string Needle;
        public string Replacer;

        public ReplaceRule(string needle, string replacer)
        {
            Needle = needle;
            Replacer = replacer;
        }

        public string Rename(string original)
        {
            string newName = original;
            if (!string.IsNullOrEmpty(Needle))
            {
                newName = original.Replace(Needle, Replacer);
            }

            return newName;
        }
    }
}

