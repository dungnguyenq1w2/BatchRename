using Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace BatchRename
{
    public class BaseWindowFactory
    {
        private static BaseWindowFactory? _instance = null;
        private Dictionary<string, BaseWindow> _windowPrototypes;
        
        BaseWindowFactory()
        {
            _windowPrototypes = new Dictionary<string, BaseWindow>();
        }

        public static BaseWindowFactory Instance()
        {
            if (_instance == null)
            {
                _instance = new BaseWindowFactory();
            }

            return _instance;
        }

        public BaseWindow GetBaseWindow(string className)
        {
            return _windowPrototypes[className];
        }

        public void Register()
        {
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var dlls = new DirectoryInfo(exeFolder).GetFiles("*.dll");
            foreach (var dll in dlls)
            {
                var assembly = Assembly.LoadFile(dll.FullName);
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass)
                    {
                        if (typeof(BaseWindow).IsAssignableFrom(type))
                        {
                            var window = Activator.CreateInstance(type) as BaseWindow;
                            _windowPrototypes.Add(window!.ClassName, window);
                        }
                    }
                }
            }
        }
    }
}
