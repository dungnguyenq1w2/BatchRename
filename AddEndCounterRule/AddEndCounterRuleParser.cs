using Contract;
using System;
using System.Windows.Controls;

namespace AddEndCounterRule
{
    public class AddEndCounterRuleParser : IRenameRuleParser
    {
        public string Name => "AddEndCounter";
        public string Title => "Add end counter";
        public bool IsPlugAndPlay => false;

        public IRenameRule Parse(string line)
        {
            string[] tokens = line.Split(new string[] { "AddEndCounter " }, StringSplitOptions.None);

            string[] inputValues = tokens[1].Split(new string[] { " " }, StringSplitOptions.None);

            int counter = int.Parse(inputValues[0]);
            int stepSize = int.Parse(inputValues[1]);
            int numberDigits = int.Parse(inputValues[2]);
            //int numberDigits = int.Parse(inputValues[2].Replace("\"", ""));

            IRenameRule rule = new AddEndCounterRule(counter, stepSize, numberDigits);

            return rule;
        }
    }
}
