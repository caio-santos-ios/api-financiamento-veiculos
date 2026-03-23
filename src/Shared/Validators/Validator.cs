using System.Text.RegularExpressions;

namespace api_financiamento.src.Shared.Validators
{
    public static partial class Validator
    {
        [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        private static partial Regex EmailRegex();

        [GeneratedRegex("[a-zA-Z]")]
        private static partial Regex LetterRegex();

        [GeneratedRegex("[A-Z]")]
        private static partial Regex UppercaseRegex();

        [GeneratedRegex("[a-z]")]
        private static partial Regex LowercaseRegex();

        [GeneratedRegex(@"\d")]
        private static partial Regex DigitRegex();

        [GeneratedRegex(@"[!@#$%^&*()_+=\-{}\[\]:;""'<>,.?\\/|]")]
        private static partial Regex SpecialRegex();

        public static bool IsEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return EmailRegex().IsMatch(email);
        }

        public static bool IsPositiveDecimal(decimal value) => value > 0;

        public static bool IsValidInstallments(int installments) =>
            new[] { 24, 36, 48, 60 }.Contains(installments);
    }
}
