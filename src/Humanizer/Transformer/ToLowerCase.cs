using System;

namespace Humanizer
{
    internal class ToLowerCase : IStringTransformer
    {
        public string Transform(ReadOnlySpan<char> input)
        {
            var copySpan = new Span<char>(new char[input.Length]);
            input.CopyTo(copySpan);

            TransformInPlace(copySpan);

            return copySpan.ToString();
        }

        public void TransformInPlace(Span<char> input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = Char.ToLower(input[i]);
            }
        }
    }
}
