using System;
using Xunit;
using Day3;

namespace Day3.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("R8,U5,L5,D3","U7,R6,D4,L4",6,30)]
        [InlineData("R75,D30,R83,U83,L12,D49,R71,U7,L72","U62,R66,U55,R34,D71,R55,D58,R83",159,610)]
        [InlineData("R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51","U98,R91,D20,R16,D67,R40,U7,R15,U6,R7",135,410)]
        public void Test1(string path1, string path2, int expectedMinDistance, int expectedMinSteps)
        {
            var impl=new Day3Impl(path1,path2);
            impl.Execute();
            Assert.Equal(expectedMinDistance,impl.ResultMinDistance);
            Assert.Equal(expectedMinSteps,impl.ResultMinSteps);
        }

        [Theory]
        [InlineData("L8",-1,0,8)]
        [InlineData("R8",1,0,8)]
        [InlineData("U8",0,1,8)]
        [InlineData("D8",0,-1,8)]
        public void TestParsePathElement(string element, int dx, int dy, int distance)
        {
            var res=WirePath.PathElement.Parse(element);
            Assert.Equal((dx,dy), res.Direction);
            Assert.Equal(distance, res.Distance);
        }
    }
}
