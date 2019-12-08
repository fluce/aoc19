using System;
using Xunit;
using Day7;
using Xunit.Abstractions;
using System.Linq;

namespace Day7.Tests
{
    public class Day7Test
    {
        readonly ITestOutputHelper console;

        public Day7Test(ITestOutputHelper console)
        {
            this.console = console;
        }

        [Theory]
        [InlineData("3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0",5,0)]
        public void TestAmplifier(string program, int phase, int input)
        {
            var amplifier=new Amplifier(program);
            amplifier.Computer.Log = Log;

            var outputSignal=amplifier.Run(phase,input);

            Assert.NotNull(outputSignal);

            console.WriteLine($"Output for phase {phase} and input {input} : {outputSignal}");            
        }

        [Theory]
        [InlineData("3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0","4,3,2,1,0",43210)]
        [InlineData("3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0","0,1,2,3,4",54321)]
        [InlineData("3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0","1,0,4,3,2",65210)]
        public void TestAmplifiersSerie(string program, string sequence, int expectedOutput)
        {
            var amplifiers=new AmplifiersSerie(program,5);
            foreach(var a in amplifiers.Amplifiers) a.Computer.Log = Log;
            amplifiers.Log=Log;

            var outputSignal=amplifiers.Run(sequence.Split(',').Select(int.Parse));

            console.WriteLine($"Output for sequence {sequence} : {outputSignal}");            

            Assert.Equal(expectedOutput,outputSignal);
        }

        [Theory]
        [InlineData("3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5","9,8,7,6,5",139629729)]
        [InlineData("3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10","9,7,8,5,6",18216)]
        public void TestAmplifiersSerieWithFeeback(string program, string sequence, int expectedOutput)
        {
            var amplifiers=new AmplifiersSerieWithFeedback(program,5);
            //foreach(var a in amplifiers.Amplifiers) a.Computer.Log = Log;
            amplifiers.Log=Log;

            var outputSignal=amplifiers.Run(sequence.Split(',').Select(int.Parse));

            console.WriteLine($"Output for sequence {sequence} : {outputSignal}");            

            Assert.Equal(expectedOutput,outputSignal);
        }


        [Theory]
        [InlineData("01","01,10")]
        [InlineData("012","012,102,201,210,021,120")]
        public void TestPermutation(string input, string expectedPermutations)
        {
            var inputAsArray = ConvertSequenceToArray(input);
            var expectedPermutationsAsArray = expectedPermutations.Split(',').OrderBy(x=>x);
            var result=Helper.GetPermutations(inputAsArray);
            var sorted_result=result.Select(ConvertArrayToSequence).OrderBy(x=>x);
            Assert.Equal(expectedPermutationsAsArray,sorted_result);
        }

        [Theory]
        [InlineData("0",1)]
        [InlineData("01",2)]
        [InlineData("012",6)]
        [InlineData("0123",24)]
        [InlineData("01234",120)]
        public void TestPermutationCount(string input, int count)
        {
            var inputAsArray = ConvertSequenceToArray(input);
            var result=Helper.GetPermutations(inputAsArray);
            Assert.Equal(count,result.Count());
        }

        private static int[] ConvertSequenceToArray(string input)
        {
            return input.Select(c => int.Parse(c.ToString())).ToArray();
        }
        private static int[] ConvertListToArray(string input)
        {
            return input.Split(',').Select(int.Parse).ToArray();
        }
        private static string ConvertArrayToSequence(int[] input)
        {
            return string.Join("",input.Select(x=>x.ToString()));
        }

        [Theory]
        [InlineData("0,1",0,"0","1")]
        [InlineData("0,1",1,"1","0")]
        [InlineData("0,1,2",0,"0","1,2")]
        [InlineData("0,1,2",1,"1","0,2")]
        [InlineData("0,1,2",2,"2","0,1")]
        public void TestSplitArray(string input, int index, string expectedPart1, string expectedPart2)
        {
            var (selectedAsFirst, remain)=Helper.SplitArray(ConvertListToArray(input),index);
            var expectedPart1AsArray=ConvertListToArray(expectedPart1);
            var expectedPart2AsArray=ConvertListToArray(expectedPart2);
            Assert.Equal(expectedPart1AsArray,selectedAsFirst);
            Assert.Equal(expectedPart2AsArray,remain);            
        }

        private void Log(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
            console.WriteLine(s);
        }
    }
}
