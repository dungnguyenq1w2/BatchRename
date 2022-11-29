using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReplaceRule
{
    /// <summary>
    /// Interaction logic for ReplaceWindow.xaml
    /// </summary>
    public partial class ReplaceWindow : BaseWindow
    {
        public override string ClassName => "Replace";
        public string Replace { get; set; }

        public ReplaceWindow()
        {
            InitializeComponent();
        }

        public override BaseWindow CreateInstance()
        {
            return new ReplaceWindow();
        }

        private void spMain_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;

            ReplaceRuleParser parser = new ReplaceRuleParser();
            if (!string.IsNullOrEmpty(Command))
            {
                ReplaceRule rule = parser.Parse(Command) as ReplaceRule;
                txtFind.Text = rule.Needle;
                Replace = rule.Replacer;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {  
            DialogResult = false;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(Replace != null && txtReplace.Text.Length <= 255)
            {
                DialogResult = true;
                Command = $"{ClassName} \"{txtFind.Text}\" => \"{Replace}\"";
            }
        }
    }
}
