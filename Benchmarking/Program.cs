using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Examples.Purity;

namespace Benchmarking
{
    class Program
    {
        static void Main()
        {
            var summary = BenchmarkRunner.Run<StringExtBenchmark>();
        }
    }

    public class StringExtBenchmark
    {
        private const string input =
            "lORem ipsUm dolor sit AMET, conSEctetur adIpisCING elit Mauris vehicula elEmenTum dolor vitae tiNCidunt.";

        [Benchmark(Baseline = true)]
        public int ToSentenceCaseInitial()
        {
            return input.ToSentenceCaseInitial().Length;
        }

        [Benchmark]
        public int ToSentenceCase()
        {
            return input.ToSentenceCase().Length;
        }
    }
}
