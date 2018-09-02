using System.Collections.Generic;
using System.Linq;
using System;

namespace Humanizer
{
    internal class ToTitleCase : IStringTransformer
    {
        public string Transform(ReadOnlySpan<char> input)
        {
            var str = input.ToString();
            return TransformLegacy(str);

            //var copySpan = new Span<char>(new char[input.Length]); // Span implementation doesn't work well yet for TitleCase
            //input.CopyTo(copySpan);

            //TransformInPlace(copySpan);

            //return copySpan.ToString();
        }

        public void TransformInPlace(Span<char> input)
        {
            throw new NotImplementedException("ToTitleCase does not support in-place transform yet.");

            //var words = input.Split(' ');

            //while (words.GetEnumerator().MoveNext())
            //{
            //    var word = words.GetEnumerator().Current;

            //    if (word.Length == 0 || AllCapitals(word))
            //    {
            //        // no-op
            //    }
            //    else if (word.Length == 1)
            //    {
            //        word[0] = Char.ToUpper(word[0]);
            //    }
            //    else
            //    {
            //        for(int i = 0; i < word.Length; i++)
            //        {
            //            if (i == 0)
            //                word[i] = Char.ToUpper(word[i]);
            //            else
            //                word[i] = Char.ToLower(word[i]);
            //        }
            //    }
            //}
        }

        //private static bool AllCapitals(Span<char> input)
        //{
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        if (Char.IsLower(input[i]))
        //            return false;
        //    }

        //    return true;
        //}

        public string TransformLegacy(string input)
        {
            var words = input.Split(' ');
            var result = new List<string>();
            foreach (var word in words)
            {
                if (word.Length == 0 || AllCapitalsLegacy(word))
                {
                    result.Add(word);
                }
                else if (word.Length == 1)
                {
                    result.Add(word.ToUpper());
                }
                else
                {
                    result.Add(char.ToUpper(word[0]) + word.Remove(0, 1).ToLower());
                }
            }

            return string.Join(" ", result);
        }

        private static bool AllCapitalsLegacy(string input)
        {
            return input.ToCharArray().All(char.IsUpper);
        }
    }
}
