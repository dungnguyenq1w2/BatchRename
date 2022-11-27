using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    public class File : INotifyPropertyChanged
    {
        private string _name = "";
        private string _newName = "";
        private string _path = "";

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public string NewName
        {
            get
            {
                return _newName;
            }
            set
            {
                _newName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewName"));
            }
        }
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Path"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
