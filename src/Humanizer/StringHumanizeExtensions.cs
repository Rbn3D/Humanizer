using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Humanizer
{
    /// <summary>
    /// Contains extension methods for humanizing string values.
    /// </summary>
    public static class StringHumanizeExtensions
    {
        private static readonly Regex PascalCaseWordPartsRegex;
        private static readonly Regex FreestandingSpacingCharRegex;

        static StringHumanizeExtensions()
        {
            PascalCaseWordPartsRegex = new Regex(@"[\p{Lu}]?[\p{Ll}]+|[0-9]+[\p{Ll}]*|[\p{Lu}]+(?=[\p{Lu}][\p{Ll}]|[0-9]|\b)|[\p{Lo}]+",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture | RegexOptionsUtil.Compiled);
            FreestandingSpacingCharRegex = new Regex(@"\s[-_]|[-_]\s", RegexOptionsUtil.Compiled);
        }

        private static string FromUnderscoreDashSeparatedWords(string input)
        {
            var span = input.AsSpan();
            var copySpan = new Span<char>(new char[input.Length]);
            span.CopyTo(copySpan);

            FromUnderscoreDashSeparatedWordsInPlace(copySpan);

            return copySpan.ToString();
        }

        private static void FromUnderscoreDashSeparatedWordsInPlace(Span<char> input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                var character = input[i];
                if (character == '_' || character == '-')
                    input[i] = ' ';
            }
        }


        private static string FromPascalCase(string input)
        {
            var result = string.Join(" ", PascalCaseWordPartsRegex
                .Matches(input).Cast<Match>()
                .Select(match => match.Value.ToCharArray().All(char.IsUpper) &&
                    (match.Value.Length > 1 || (match.Index > 0 && input[match.Index - 1] == ' ') || match.Value == "I")
                    ? match.Value
                    : match.Value.ToLower()));

            return result.Length > 0 ? char.ToUpper(result[0]) +
                result.Substring(1, result.Length - 1) : result;
        }

        private static Span<char> FromPascalCase(this ReadOnlySpan<char> input)
        {
            var countSpaces = 0;
            var currentIndexSpace = 0;

            var countIgnoreChars = 0;
            var currentIndexIgnore = 0;

            for (int x = 1; x < input.Length; x++)
            {
                if (Char.IsUpper(input[x]) || (Char.IsDigit(input[x]) && !Char.IsDigit(input[x - 1])))
                {
                    countSpaces++;
                }

                if(Char.IsWhiteSpace(input[x]) || input[x] == '?' || input[x] == '(' || input[x] == ')' || input[x] == '@')
                {
                    countIgnoreChars++;
                }
            }

            Span<int> spaceOccurrences = new Span<int>(new int[countSpaces]);
            Span<int> ignoreOccurrences = new Span<int>(new int[countIgnoreChars]);

            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsWhiteSpace(input[i]) || input[i] == '?' || input[i] == '(' || input[i] == ')' || input[i] == '@')
                {
                    ignoreOccurrences[currentIndexIgnore] = i;
                }
                else if (i > 0 && Char.IsUpper(input[i]) || (i > 0 && Char.IsDigit(input[i]) && !Char.IsDigit(input[i - 1])))
                {
                    spaceOccurrences[currentIndexSpace] = i;
                    currentIndexSpace++;
                }
                
            }

            var writableSpan = new Span<char>(new char[input.Length + countSpaces - countIgnoreChars]);

            var enumerator = writableSpan.GetEnumerator();
            int idxInput = -1;
            int spaceIdx = 0;
            int ignoreIdx = 0;

            while(enumerator.MoveNext())
            {
                idxInput++;

                if(!((ignoreIdx <= (ignoreOccurrences.Length - 1)) && idxInput == ignoreOccurrences[ignoreIdx]))
                {
                    if ((spaceIdx <= (spaceOccurrences.Length - 1)) && idxInput == spaceOccurrences[spaceIdx])
                    {
                        spaceIdx++;
                        idxInput--;
                        enumerator.Current = ' ';
                    }
                    else if (idxInput <= (input.Length - 1))
                        enumerator.Current = input[idxInput];
                }
            }

            for (int i = 0; i < writableSpan.Length; i++)
            {
                var isIUpper = (writableSpan[i] == 'I' || writableSpan[i] == 'i') && (i == 0 || Char.IsWhiteSpace(writableSpan[i - 1])) && (i >= (writableSpan.Length - 1) || Char.IsWhiteSpace(writableSpan[i + 1]));

                if (i == 0 || isIUpper)
                    writableSpan[i] = Char.ToUpper(writableSpan[i]);
                else
                    writableSpan[i] = Char.ToLower(writableSpan[i]);
            }

            return writableSpan;
        }


        /// <summary>
        /// Humanizes the input string; e.g. Underscored_input_String_is_turned_INTO_sentence -> 'Underscored input String is turned INTO sentence'
        /// </summary>
        /// <param name="input">The string to be humanized</param>
        /// <returns></returns>
        public static string Humanize(this string input)
        {
            // if input is all capitals (e.g. an acronym) then return it without change
            if (input.ToCharArray().All(char.IsUpper))
            {
                return input;
            }

            // if input contains a dash or underscore which preceeds or follows a space (or both, e.g. free-standing)
            // remove the dash/underscore and run it through FromPascalCase
            if (FreestandingSpacingCharRegex.IsMatch(input))
            {
                return FromPascalCase(FromUnderscoreDashSeparatedWords(input));
            }

            if (input.Contains("_") || input.Contains("-"))
            {
                return FromUnderscoreDashSeparatedWords(input);
            }

            return FromPascalCase(input);
        }

        /// <summary>
        /// Humanizes the input string; e.g. Underscored_input_String_is_turned_INTO_sentence -> 'Underscored input String is turned INTO sentence'
        /// This overload should perfrom better than the one accepting a string
        /// 
        /// </summary>
        /// <param name="input">The Span to be humanized</param>
        /// <returns>An span representing the Humanized string. Call ToString() to get an String from it</returns>
        public static Span<char> Humanize(this Span<char> input)
        {
            // if input is all capitals (e.g. an acronym) then return it without change
            if (AllCapitals(input))
            {
                return input;
            }

            // if input contains a dash or underscore which preceeds or follows a space (or both, e.g. free-standing)
            // remove the dash/underscore and run it through FromPascalCase
            if (FreestandingSpacingCharRegex.IsMatch(input.ToString()))
            {
                FromUnderscoreDashSeparatedWordsInPlace(input);
                input = FromPascalCase(input);

                return input;
            }

            if ((input.IndexOf('_') != -1) || (input.IndexOf('-') != -1))
            {
                FromUnderscoreDashSeparatedWordsInPlace(input);
                return input;
            }

            return FromPascalCase(input);
        }

        /// <summary>
        /// Humanized the input string based on the provided casing
        /// </summary>
        /// <param name="input">The string to be humanized</param>
        /// <param name="casing">The desired casing for the output</param>
        /// <returns></returns>
        public static string Humanize(this string input, LetterCasing casing)
        {
            return input.Humanize().ApplyCase(casing);
        }

        /// <summary>
        /// Humanized the input string based on the provided casing
        /// </summary>
        /// <param name="input">The string to be humanized</param>
        /// <param name="casing">The desired casing for the output</param>
        /// <returns></returns>
        public static void Humanize(this Span<char> input, LetterCasing casing)
        {
            input.ApplyCaseInPlace(casing);
        }

        private static bool AllCapitals(Span<char> input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLower(input[i]))
                    return false;
            }

            return true;
        }
    }
}
