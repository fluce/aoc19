using System;
using Xunit;
using Xunit.Abstractions;

namespace Day11.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper console;

        public UnitTest1(ITestOutputHelper console)
        {
            this.console = console;
        }


        [Fact]
        public void TestInitHull()
        {
            var h=new Hull(5,5);
            console.WriteLine($"Init {h.Position} {h.Direction}");
            console.WriteLine(h.Dump());
            Assert.Equal((2,2),h.Position);
            Assert.Equal(0, h.Direction);

        }

        [Fact]
        public void TestStep0()
        {
            var h=new Hull(5,5);
            h.OneStep('#',0);
            console.WriteLine($"Step1 {h.Position} {h.Direction}");
            console.WriteLine(h.Dump());
            Assert.Equal((1,2),h.Position);
            Assert.Equal(3, h.Direction);
            Assert.Equal('#',h.Panels[2,2]);

        }

        [Fact]
        public void TestStep1()
        {
            var h=new Hull(5,5);
            h.OneStep('#',1);
            console.WriteLine($"Step1 {h.Position} {h.Direction}");
            console.WriteLine(h.Dump());
            Assert.Equal((3,2),h.Position);
            Assert.Equal(1, h.Direction);
            Assert.Equal('#',h.Panels[2,2]);

        }

        [Fact]
        public void TestStep11()
        {
            var h=new Hull(5,5);
            h.OneStep('#',1);
            h.OneStep('#',1);
            console.WriteLine($"Step1 {h.Position} {h.Direction}");
            console.WriteLine(h.Dump());
            Assert.Equal((3,3),h.Position);
            Assert.Equal(2, h.Direction);
            Assert.Equal('#',h.Panels[2,2]);
            Assert.Equal('#',h.Panels[3,2]);

        }

    }
}
