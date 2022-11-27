using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Contract;

namespace BatchRename
{
    public class RenameRuleParserFactory
    {
        private static RenameRuleParserFactory _instance = new RenameRuleParserFactory();
        private Dictionary<string, IRenameRuleParser> _ruleParserPrototypes;
        private Dictionary<string, BaseWindow> _windowPrototypes;
        public Dictionary<string, IRenameRuleParser> RuleParserPrototypes { get => _ruleParserPrototypes; }
        public Dictionary<string, BaseWindow> WindowPrototypes { get => WindowPrototypes; }
        RenameRuleParserFactory()
        {
            _ruleParserPrototypes = new Dictionary<string, IRenameRuleParser>();
            _windowPrototypes = new Dictionary<string, BaseWindow>();
        }

        public static RenameRuleParserFactory Instance()
        {
            return _instance;
        }

        public IRenameRuleParser GetRuleParser(string ruleParserName)
        {
            return _ruleParserPrototypes[ruleParserName];
        }

        public BaseWindow GetWindow(string windowName)
        {
            return _windowPrototypes[windowName];
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
                        if (typeof(IRenameRuleParser).IsAssignableFrom(type))
                        {
                            var ruleParser = Activator.CreateInstance(type) as IRenameRuleParser;
                            _ruleParserPrototypes.Add(ruleParser!.Name, ruleParser);
                        }
                        if (typeof(BaseWindow).IsAssignableFrom(type))
                        {
                            var window = Activator.CreateInstance(type) as BaseWindow;
                            _windowPrototypes.Add(window.ClassName, window);
                        }
                    }
                }
            }
        }
    }
}
