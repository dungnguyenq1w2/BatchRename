using Contract;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;

namespace AddEndCounterRule
{
    public class AddEndCounterRule : IRenameRule
    {
        public int Counter;

        public int StepSize;

        //public int NextCounter;

        public int NumberDigits;

        public AddEndCounterRule(int counter) { }

        public AddEndCounterRule(int counter, int stepSize, int numberDigits)
        {
            Counter = counter;
            StepSize = stepSize;
            //NextCounter = nextCounter;
            NumberDigits = numberDigits;
        }

        public string Rename(string original)
        {
            string formatCounter = FormatCounter();
            string newName = $"{Path.GetFileNameWithoutExtension(original)}{formatCounter}{Path.GetExtension(original)}";

            Counter += StepSize;

            return newName;
        }

        // add padding to Counter
        public string FormatCounter()
        {
            string formatCounter = Counter.ToString().PadLeft(NumberDigits, '0');
            return formatCounter;
        }

        //public List<string> RenameList(List<string> originals)
        //{
        //    List<string> newNames = new List<string>();
        //    foreach (string original in originals)
        //    {
        //        string newName = Rename(original);
        //        newNames.Add(newName);
        //    }

        //    return newNames;
        //}
    }
}
