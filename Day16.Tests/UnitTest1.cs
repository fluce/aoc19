using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Day16.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper console;

        public UnitTest1(ITestOutputHelper console)
        {
            this.console = console;
        }



        [Fact]
        public void TestApplyPattern()
        {
            var ret=FFT.ApplyPattern(new byte[]{ 1,2,3 },0);
            Assert.Equal(2,ret);
        }

        [Fact]
        public void TestCalcPatternIndex0()
        {
            var ret=FFT.CalcPattern(new int[] { 0,1,2 },0).Take(6).ToArray();
            Assert.Equal(new int[] { 0,1,2,0,1,2 }, ret);
        }

        [Fact]
        public void TestCalcPatternIndex1()
        {
            var ret=FFT.CalcPattern(new int[] { 0,1,2 },1).Take(12).ToArray();
            Assert.Equal(new int[] { 0,0,1,1,2,2,0,0,1,1,2,2 }, ret);
        }

        [Theory]
        [InlineData("12345678",1,"48226158")]
        [InlineData("48226158",1,"34040438")]
        [InlineData("34040438",1,"03415518")]
        [InlineData("03415518",1,"01029498")]
        [InlineData("80871224585914546619083218645595",100,"24176176")]
        [InlineData("19617804207202209144916044189917",100,"73745418")]
        [InlineData("69317163492948606335995924319873",100,"52432133")]
        public void TestApply(string input, int phaseCount, string expected)
        {
            //FFT.Log=console.WriteLine;
            var fft=new FFT();
            var ret=fft.Apply(input,phaseCount);
            Assert.Equal(expected,ret);
        }

        [Theory]
        [InlineData("03036732577212944063491565474664","84462026")]
        [InlineData("02935109699940807407585447034323","78725270")]
        [InlineData("03081770884921959731165446850517","53553731")]
        public void TestApplyFull(string input, string expected)
        {
            FFT.Log=console.WriteLine;
            var fft=new FFT();
            var ret=fft.ApplyFull(input);
            Assert.Equal(expected,ret);
        }

    }
}
