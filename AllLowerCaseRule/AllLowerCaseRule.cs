using Contract;
using System;

namespace AllLowerCaseRule
{
    public class AllLowerCaseRule : IRenameRule
    {
        public string Rename(string original)
        {
            return original.ToLower().Replace(" ", "");
        }
    }
}
