using Contract;
using System.Windows;

namespace AddEndCounterRule
{
    /// <summary>
    /// Interaction logic for AddSuffixWindow.xaml
    /// </summary>
    public partial class AddEndCounterWindow : BaseWindow
    {
        public override string ClassName => "AddEndCounter";
        public int Counter { get; set; } = 1;
        public int StepSize { get; set; } = 1;
        public int NumberDigits { get; set; } = 2;

        public AddEndCounterWindow()
        {
            InitializeComponent();
        }

        public override BaseWindow CreateInstance()
        {
            return new AddEndCounterWindow();
        }

        private void spMain_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;

            AddEndCounterRuleParser parser = new AddEndCounterRuleParser();
            if (!string.IsNullOrEmpty(Command))
            {
                AddEndCounterRule rule = parser.Parse(Command) as AddEndCounterRule;
                Counter = rule.Counter;
                StepSize = rule.StepSize;
                NumberDigits = rule.NumberDigits;

                // Counter = rule.Counter;
                // StepSize = rule.StepSize;
                // NumberDigits = rule.NumberDigits;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Command = $"{ClassName} {Counter} {StepSize} {NumberDigits}";
            DialogResult = true;
        }
    }
}
