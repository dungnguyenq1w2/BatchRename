using System.Windows;

namespace Contract
{
    public class BaseWindow : Window
    {
        public virtual string ClassName { get; }
        public virtual string Command { get; set; }

        public virtual BaseWindow CreateInstance()
        {
            return new BaseWindow();
        }
    }
}
