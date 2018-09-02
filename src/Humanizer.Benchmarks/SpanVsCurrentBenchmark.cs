extern alias HumanizerSpanRef;

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using HumanizerRegular = global::Humanizer;
using HumanizerSpan = HumanizerSpanRef.Humanizer;

namespace Humanizer.Benchmarks
{
    public class SpanVsCurrentBenchmark
    {
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void HumanizeString(String str)
        {
            HumanizerRegular.StringHumanizeExtensions.Humanize(str);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void HumanizeStringSpan(String str)
        {
            HumanizerSpan.StringHumanizeExtensions.Humanize(str);
        }

        public IEnumerable<object[]> Data()
        {
            yield return new object[] { "PascalCaseInputStringIsTurnedIntoSentence" };
            yield return new object[] { "WhenIUseAnInputAHere" };
            yield return new object[] { "10IsInTheBegining" };
            yield return new object[] { "NumberIsFollowedByLowerCase5th" };
            yield return new object[] { "NumberIsAtTheEnd100" };
            yield return new object[] { "XIsFirstWordInTheSentence" };
            yield return new object[] { "XIsFirstWordInTheSentence" };
            yield return new object[] { "ContainsSpecial" };
            yield return new object[] { "a" };
            yield return new object[] { "A" };
            yield return new object[] { "?" };
            yield return new object[] { "?" };
            yield return new object[] { "" };
            yield return new object[] { "JeNeParlePasFrançais" };
        }
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SpanVsCurrentBenchmark>();
        }
    }
}
