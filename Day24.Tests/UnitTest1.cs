using System;
using Xunit;

namespace Day24.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestBioDiversityRate()
        {
             var input =@".....
.....
.....
#....
.#...";
            var lifeGame=new LifeGame(input);
            Assert.Equal(2129920,lifeGame.BioDiversityRating);
        }

        [Fact]
        public void TestIterate()
        {
            var input=@"....#
#..#.
#.?##
..#..
#....";
            var lifeGame=new LifeGame(input);

            for (int i=0;i<10;i++)
            {
                lifeGame.Iterate2();
            }
            Assert.Equal(99,lifeGame.BugCount);
        }
    }
}
