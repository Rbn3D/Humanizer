using System;

namespace Humanizer
{
    /// <summary>
    /// ApplyCase method to allow changing the case of a sentence easily
    /// </summary>
    public static class CasingExtensions
    {
        /// <summary>
        /// Changes the casing of the provided input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="casing"></param>
        /// <returns></returns>
        public static string ApplyCase(this string input, LetterCasing casing)
        {
            switch (casing)
            {
                case LetterCasing.Title:
                    return input.Transform(To.TitleCase);

                case LetterCasing.LowerCase:
                    return input.Transform(To.LowerCase);

                case LetterCasing.AllCaps:
                    return input.Transform(To.UpperCase);

                case LetterCasing.Sentence:
                    return input.Transform(To.SentenceCase);

                default:
                    throw new ArgumentOutOfRangeException(nameof(casing));
            }
        }

        /// <summary>
        /// Changes the case of the provided input in-place
        /// </summary>
        /// <param name="input"></param>
        /// <param name="casing"></param>
        public static void ApplyCaseInPlace(this Span<char> input, LetterCasing casing)
        {
            switch (casing)
            {
                case LetterCasing.Title:
                    input.TransformInPlace(To.TitleCase);
                    break;
                case LetterCasing.LowerCase:
                    input.TransformInPlace(To.LowerCase);
                    break;
                case LetterCasing.AllCaps:
                    input.TransformInPlace(To.UpperCase);
                    break;
                case LetterCasing.Sentence:
                    input.TransformInPlace(To.SentenceCase);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(casing));
            }
        }
    }
}
