using System;

namespace Contract
{
    public class BaseWindow
    {
        public virtual string ClassName { get; }
        public virtual string Command { get; set; }

        public virtual BaseWindow CreateInstance()
        {
            return new BaseWindow();
        }
    }
}
