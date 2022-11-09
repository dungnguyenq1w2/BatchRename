using Contract;
using System;

namespace RemoveAllSpaceRule
{
    public class RemoveAllSpaceRule : IRenameRule
    {
        public string Rename(string original)
        {
            return original.Replace(" ", "");
        }
    }
}
