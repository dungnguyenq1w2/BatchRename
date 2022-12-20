using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    public class WorkingCondition
    {
        public List<string> Preset { get; set; } = new List<string>();
        public List<string> ActiveFiles { get; set; } = new List<string>();
        public List<string> ActiveFolders { get; set; } = new List<string>();
        public double Top { get; set; } = 0;
        public double Left { get; set; } = 0;
        public double Height { get; set; } = 450;
        public double Width { get; set; } = 800;
        public bool Maximized { get; set; } = false;

        public void Reset()
        {
            Preset.Clear();
            ActiveFiles.Clear();
            ActiveFolders.Clear();
            Top = 0;
            Left = 0;
            Height = 450;
            Width = 800;
            Maximized = false;
        }
    }
}
