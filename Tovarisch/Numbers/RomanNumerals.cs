namespace Wacton.Tovarisch.Numbers
{
    using System;

    public static class RomanNumerals
    {
        // see: http://stackoverflow.com/questions/7040289/converting-integers-to-roman-numerals
        public static string ToRomanNumerals(this int number)
        {
            if ((number < 0) || (number > 3999))
            {
                throw new ArgumentOutOfRangeException("Can only represent numbers between 0 and 3999");
            }

            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRomanNumerals(number - 1000);
            if (number >= 900) return "CM" + ToRomanNumerals(number - 900);
            if (number >= 500) return "D" + ToRomanNumerals(number - 500);
            if (number >= 400) return "CD" + ToRomanNumerals(number - 400);
            if (number >= 100) return "C" + ToRomanNumerals(number - 100);
            if (number >= 90) return "XC" + ToRomanNumerals(number - 90);
            if (number >= 50) return "L" + ToRomanNumerals(number - 50);
            if (number >= 40) return "XL" + ToRomanNumerals(number - 40);
            if (number >= 10) return "X" + ToRomanNumerals(number - 10);
            if (number >= 9) return "IX" + ToRomanNumerals(number - 9);
            if (number >= 5) return "V" + ToRomanNumerals(number - 5);
            if (number >= 4) return "IV" + ToRomanNumerals(number - 4);
            if (number >= 1) return "I" + ToRomanNumerals(number - 1);

            throw new InvalidOperationException(string.Format("Unable to convert {0} to roman numeral", number));
        }
    }
}
