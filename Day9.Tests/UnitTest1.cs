using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Day9.Tests
{
    public class IntCodeComputerDay2Test
    {
        private readonly ITestOutputHelper console;

        public IntCodeComputerDay2Test(ITestOutputHelper console)
        {
            this.console = console;
        }

        [Theory]
        [InlineData("1,0,0,0,99","2,0,0,0,99")]
        [InlineData("2,3,0,3,99","2,3,0,6,99")]
        [InlineData("2,4,4,5,99,0","2,4,4,5,99,9801")]
        [InlineData("1,1,1,4,99,5,6,0,99","30,1,1,4,2,5,6,0,99")]
        [InlineData("1,9,10,3,2,3,11,0,99,30,40,50","3500,9,10,70,2,3,11,0,99,30,40,50")]
        public void Test1(string input, string output)
        {
            var impl=new IntCodeComputer(input);
            impl.Log=(x)=>console.WriteLine(x);
            impl.Execute();
            Assert.Equal(output,impl.Output);
        }

        [Fact]
        public void Test2()
        {
            var program="1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,1,10,19,2,6,19,23,1,23,5,27,1,27,13,31,2,6,31,35,1,5,35,39,1,39,10,43,2,6,43,47,1,47,5,51,1,51,9,55,2,55,6,59,1,59,10,63,2,63,9,67,1,67,5,71,1,71,5,75,2,75,6,79,1,5,79,83,1,10,83,87,2,13,87,91,1,10,91,95,2,13,95,99,1,99,9,103,1,5,103,107,1,107,10,111,1,111,5,115,1,115,6,119,1,119,10,123,1,123,10,127,2,127,13,131,1,13,131,135,1,135,10,139,2,139,6,143,1,143,9,147,2,147,6,151,1,5,151,155,1,9,155,159,2,159,6,163,1,163,2,167,1,10,167,0,99,2,14,0,0";  
            var impl=new IntCodeComputer(program,1202);
            impl.Log=(x)=>console.WriteLine(x);
            impl.Execute();
            Assert.Equal(5098658,impl.Result);
        }
    }

    public class IntCodeComputerDay5Test
    {
        private readonly ITestOutputHelper console;

        public IntCodeComputerDay5Test(ITestOutputHelper console)
        {
            this.console = console;
        }

        [Theory]
        [InlineData("1001,4,3,4,96","1001,4,3,4,99")]
        [InlineData("1001,4,-3,4,102","1001,4,-3,4,99")]
        [InlineData("1002,4,3,4,33","1002,4,3,4,99")]
        [InlineData("0101,3,4,4,96","101,3,4,4,99")]
        [InlineData("0102,3,4,4,33","102,3,4,4,99")]
        public void TestIndirect(string program, string expected_result)
        {
            var impl=new IntCodeComputer(program);
            impl.Log=(x)=>console.WriteLine(x);
            impl.Execute();
            Assert.Equal(expected_result, impl.Output);
        }

        [Theory]

        [InlineData("3,0,4,0,99",5,5)]

        [InlineData("3,9,8,9,10,9,4,9,99,-1,8",8,1)]
        [InlineData("3,9,8,9,10,9,4,9,99,-1,8",5,0)]
        [InlineData("3,9,7,9,10,9,4,9,99,-1,8",7,1)]
        [InlineData("3,9,7,9,10,9,4,9,99,-1,8",8,0)]
        [InlineData("3,9,7,9,10,9,4,9,99,-1,8",9,0)]

        [InlineData("3,3,1108,-1,8,3,4,3,99",8,1)]
        [InlineData("3,3,1108,-1,8,3,4,3,99",5,0)]
        [InlineData("3,3,1107,-1,8,3,4,3,99",7,1)]
        [InlineData("3,3,1107,-1,8,3,4,3,99",8,0)]
        [InlineData("3,3,1107,-1,8,3,4,3,99",9,0)]

        [InlineData("3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9",0,0)]
        [InlineData("3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9",1,1)]
        [InlineData("3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9",5,1)]

        [InlineData("3,3,1105,-1,9,1101,0,0,12,4,12,99,1",0,0)]
        [InlineData("3,3,1105,-1,9,1101,0,0,12,4,12,99,1",1,1)]
        [InlineData("3,3,1105,-1,9,1101,0,0,12,4,12,99,1",5,1)]

        [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99",-30,999)]
        [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99",7,999)]
        [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99",8,1000)]
        [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99",9,1001)]
        [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99",50,1001)]

        public void TestInputOutput(string program, int input, int expected_output)
        {
            var impl=new IntCodeComputer(program);
            impl.GetNextInput=()=>input;
            impl.Log=(x)=>console.WriteLine(x);
            long? output=null;
            impl.SetOutput=x=>output=x;
            impl.Execute();
            Assert.Equal(expected_output, output);
        }
    }

    public class IntCodeComputerDay9Test
    {
        private readonly ITestOutputHelper console;

        public IntCodeComputerDay9Test(ITestOutputHelper console)
        {
            this.console = console;
        }

        [Fact]
        public void TestRelative()
        {
            var program="109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            var impl=new IntCodeComputer(program);
            var output=new List<long>();
            impl.Log=(x)=>console.WriteLine(x);
            impl.SetOutput=x=>output.Add(x.Value);
            impl.Execute();
            Assert.Equal(program, string.Join(",",output.Select(x=>x.ToString())));
        }

        [Fact]
        public void TestLongMul()
        {
            var program="1102,34915192,34915192,7,4,7,99,0";
            var impl=new IntCodeComputer(program);
            var output=new List<long>();
            impl.Log=(x)=>console.WriteLine(x);
            impl.SetOutput=x=>output.Add(x.Value);
            impl.Execute();
            Assert.Single(output);
            Assert.Equal(16, output.Single().ToString().Length);
        }

        [Fact]
        public void TestLongOutput()
        {
            var program="104,1125899906842624,99";
            var impl=new IntCodeComputer(program);
            var output=new List<long>();
            impl.Log=(x)=>console.WriteLine(x);
            impl.SetOutput=x=>output.Add(x.Value);
            impl.Execute();
            Assert.Single(output);
            Assert.Equal("1125899906842624", output.Single().ToString());
        }

        [Fact]
        public void TestRelativeResult()
        {
            var program="109,10,203,0,99";
            var impl=new IntCodeComputer(program);
            var output=new List<long>();
            impl.Log=(x)=>console.WriteLine(x);
            impl.GetNextInput=()=>999;
            impl.Execute();
            Assert.Equal(999, impl.Memory[10]);
        }

    }
}
