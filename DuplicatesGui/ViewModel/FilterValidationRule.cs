using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace DuplicatesGui.ViewModel
{
    public class FilterValidationRule : ValidationRule
    {
        public Type ValidationType { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var arg = value as string;
            var tokens = arg.Split(';');
            foreach(var tok in tokens)
            {
                Regex regex = new Regex("([*].+)");
                if(!regex.Match(tok).Success)
                {
                    return new ValidationResult(false, $"Invalid token '{tok}'");
                }
            }
            return new ValidationResult(true, null);
        }
    }
}
