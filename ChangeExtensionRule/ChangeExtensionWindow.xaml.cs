using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChangeExtensionRule
{
    /// <summary>
    /// Interaction logic for ChangeExtensionWindow.xaml
    /// </summary>
    public partial class ChangeExtensionWindow : BaseWindow
    {
        public override string ClassName => "ChangeExtension";
        public string Extension { get; set; }

        public ChangeExtensionWindow()
        {
            InitializeComponent();
        }

        public override BaseWindow CreateInstance()
        {
            return new ChangeExtensionWindow();
        }

        private void spMain_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;

            ChangeExtensionRuleParser parser = new ChangeExtensionRuleParser();
            if (!string.IsNullOrEmpty(Command))
            {
                ChangeExtensionRule rule = parser.Parse(Command) as ChangeExtensionRule;
                Extension = rule.Extension;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Command = $"{ClassName} {Extension}";
            DialogResult = true;
        }
    }
}
