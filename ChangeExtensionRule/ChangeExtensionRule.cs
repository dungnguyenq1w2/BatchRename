using Contract;
using System.IO;

namespace ChangeExtensionRule
{
    public class ChangeExtensionRule : IRenameRule
    {
        public string Extension { get; }

        public ChangeExtensionRule(string extension)
        {
            Extension = extension;
        }

        public string Rename(string original)
        {
            string newName = original;
            if (!string.IsNullOrEmpty(Extension))
            {
                newName = Path.GetFileNameWithoutExtension(original) + "." + Extension;
            }

            return newName;
        }
    }
}