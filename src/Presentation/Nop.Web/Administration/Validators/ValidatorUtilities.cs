using System.Linq;

namespace Nop.Admin.Validators
{
    public class ValidatorUtilities
    {
        public static bool PageSizeOptionsValidator(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }
            var notValid = input.Split(',').Select(p => p.Trim()).GroupBy(p => p).Any(p => p.Count() > 1);
            return !notValid;
        }
    }
}