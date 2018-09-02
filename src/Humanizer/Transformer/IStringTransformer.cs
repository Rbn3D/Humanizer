using System;

namespace Humanizer
{
    /// <summary>
    /// Can tranform a string
    /// </summary>
    public interface IStringTransformer
    {
        /// <summary>
        /// Transform the input
        /// </summary>
        /// <param name="input">ReadOnlySpan/String to be transformed</param>
        /// <returns></returns>
        string Transform(ReadOnlySpan<char> input);

        /// <summary>
        /// Transforms the input in place, using a Span
        /// </summary>
        /// <param name="input">Span to be transformed</param>
        void TransformInPlace(Span<char> input);
    }
}
