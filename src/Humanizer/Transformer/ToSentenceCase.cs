using System;

namespace Humanizer
{
    internal class ToSentenceCase : IStringTransformer
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
            if (input.Length > 0)
                input[0] = Char.ToUpper(input[0]);
        }
    }
}
