using Contract;
using System.Windows;

namespace AddSuffixRule
{
    /// <summary>
    /// Interaction logic for AddSuffixWindow.xaml
    /// </summary>
    public partial class AddSuffixWindow : BaseWindow
    {
        public override string ClassName => "AddSuffix";
        public string Suffix { get; set; }

        public AddSuffixWindow()
        {
            InitializeComponent();
        }

        public override BaseWindow CreateInstance()
        {
            return new AddSuffixWindow();
        }

        private void spMain_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;

            AddSuffixRuleParser parser = new AddSuffixRuleParser();
            if (!string.IsNullOrEmpty(Command))
            {
                AddSuffixRule rule = parser.Parse(Command) as AddSuffixRule;
                Suffix = rule.Suffix;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Command = $"{ClassName} {Suffix}";
            DialogResult = true;
        }
    }
}
