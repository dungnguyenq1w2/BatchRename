using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    public class RunRule
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsPlugAndPlay { get; set; }
        public string Command { get; set; }
    }
}
