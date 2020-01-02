using System;
using Xunit;

namespace Day18.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(@"########################
#f.D.E.e.C.b.A.@.a.B.c.#
######################.#
#d.....................#
########################",86)]
        [InlineData(@"########################
#...............b.C.D.f#
#.######################
#.....@.a.B.c.d.A.e.F.g#
########################",132)]
        [InlineData(@"#################
#i.G..c...e..H.p#
########.########
#j.A..b...f..D.o#
########@########
#k.E..a...g..B.n#
########.########
#l.F..d...h..C.m#
#################",136)]
        [InlineData(@"########################
#@..............ac.GI.b#
###d#e#f################
###A#B#C################
###g#h#i################
########################",81)]
        public void TestSHortest(string data, int expected)
        {
            Maze m=new Maze(data.Split(new[]{'\n','\r'}, StringSplitOptions.RemoveEmptyEntries));
            var dist=m.CalcShortestPathLength();
            Assert.Equal(expected,dist);
        }
    }
}
