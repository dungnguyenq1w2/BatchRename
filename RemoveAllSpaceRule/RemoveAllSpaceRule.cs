using Contract;
using System.IO;

namespace RemoveAllSpaceRule
{
    public class RemoveAllSpaceRule : IRenameRule
    {
        public string Rename(string original)
        {
            return Path.GetFileNameWithoutExtension(original).Trim() + Path.GetExtension(original).Trim();
        }
    }
}
