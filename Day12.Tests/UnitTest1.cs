using System;
using System.Linq;
using Xunit;

namespace Day12.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("<x=-8, y=-10, z=0>",-8,-10,0)]
        [InlineData("<x=9, y=8, z=-3>",9,8,-3)]
        public void TestParse(string line, int x,int y, int z)
        {
            var m=Moon.ParseInput(line);
            Assert.Equal(x,m.X);
            Assert.Equal(y,m.Y);
            Assert.Equal(z,m.Z);
            Assert.Equal(0,m.vX);
            Assert.Equal(0,m.vY);
            Assert.Equal(0,m.vZ);
        }

        [Theory]
        [InlineData(3,5,0,0,1,-1)]
        [InlineData(5,3,0,0,-1,1)]
        [InlineData(3,3,0,0,0,0)]
        public void TestGravity(int d1, int d2, int vd1, int vd2, int nv1, int nv2)
        {
            JupiterSystem.ApplyGravity(d1,d2, ref vd1, ref vd2);
            Assert.Equal(nv1,vd1);
            Assert.Equal(nv2,vd2);
        }

        [Fact] 
        public void TestGetPair()
        {
            var system=new JupiterSystem(@"<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>");
            Assert.Equal(6,system.GetPairs().Count());
        }

        [Fact]
        public void TestOneStep()
        {
            //System.Diagnostics.Debugger.Launch();
            var system=new JupiterSystem(@"<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>");
            system.SimulateTimeStep();
            Assert.Equal(new[] { 
                Moon.ParseInput("pos=<x= 2, y=-1, z= 1>, vel=<x= 3, y=-1, z=-1>"),
                Moon.ParseInput("pos=<x= 3, y=-7, z=-4>, vel=<x= 1, y= 3, z= 3>"),
                Moon.ParseInput("pos=<x= 1, y=-7, z= 5>, vel=<x=-3, y= 1, z=-3>"),
                Moon.ParseInput("pos=<x= 2, y= 2, z= 0>, vel=<x=-1, y=-3, z= 1>")
             },system.Moons);
        }

        [Fact]
        public void TestSimulation()
        {
            //System.Diagnostics.Debugger.Launch();
            var system=new JupiterSystem(@"<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>");
            system.SimulateTimeStep(10);
            Assert.Equal(new[] { 
                Moon.ParseInput("pos=<x= 2, y= 1, z=-3>, vel=<x=-3, y=-2, z= 1>"),
                Moon.ParseInput("pos=<x= 1, y=-8, z= 0>, vel=<x=-1, y= 1, z= 3>"),
                Moon.ParseInput("pos=<x= 3, y=-6, z= 1>, vel=<x= 3, y= 2, z=-3>"),
                Moon.ParseInput("pos=<x= 2, y= 0, z= 4>, vel=<x= 1, y=-1, z=-1>")
             },system.Moons);

            Assert.Equal(179, system.GetTotalEnergy());
        }

    }
}
