using System.IO;
using System.Globalization;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Contract
{
    public class FileNameRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string segment = (string)value;
            
            string regexInvalidFileNameChars = @"^[\w\-. ]+$";
            string regexInvalidFileNameLength = @"^(?=.{1,15}$).*";

            Regex regexChars = new Regex(regexInvalidFileNameChars);
            Regex regexLength = new Regex(regexInvalidFileNameLength);

            Match resultChars = regexChars.Match(segment);
            Match resultLength = regexLength.Match(segment);

            //segment.IndexOfAny(Path.GetInvalidFileNameChars()) != -1

            if (!resultChars.Success)
            {
                return new ValidationResult(
                    false,
                    @"Can't contain any of the characters: \ / : * ? "" < > |"
                );
            }
            else if (!resultLength.Success)
            {
                return new ValidationResult(
                    false,
                    @"Length of file should limit between 1-15 chars"
                );
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
