using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Day9.Tests
{

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
