using System;
using Xunit;
using Xunit.Abstractions;
using Day10;

namespace Day10.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper console;

        public UnitTest1(ITestOutputHelper console)
        {
            this.console = console;
        }


        [Theory]
        [InlineData(@".#..#,.....,#####,....#,...##", 3, 4, 8)]
        [InlineData(@".#..#,.....,#####,....#,...##", 4, 4, 7)]
        [InlineData(@".#..#,.....,#####,....#,...##", 4, 3, 7)]
        [InlineData(@".#..#,.....,#####,....#,...##", 4, 2, 5)]
        [InlineData(@".#..#,.....,#####,....#,...##", 3, 2, 7)]
        [InlineData(@".#..#,.....,#####,....#,...##", 2, 2, 7)]
        [InlineData(@".#..#,.....,#####,....#,...##", 1, 2, 7)]
        [InlineData(@".#..#,.....,#####,....#,...##", 0, 2, 6)]
        [InlineData(@".#..#,.....,#####,....#,...##", 1, 0, 7)]
        [InlineData(@".#..#,.....,#####,....#,...##", 4, 0, 7)]
        public void Test1(string map, int fromx, int fromy, int expectedCount)
        {
            //System.Diagnostics.Debugger.Launch();
            var field=new AsteroidField(map.Split(','));
            //field.Log=console.WriteLine;

            var c=field.CountUnblockedAsteroid(field.GetCell(null,fromx,fromy));
            if (c!=expectedCount) {
                field.Log=console.WriteLine;
                field.CountUnblockedAsteroid(field.GetCell(null,fromx,fromy));
            }


            Assert.Equal(expectedCount,c);

        }

        [Fact]
        public void Test2()
        {
            var map=@".#..#,.....,#####,....#,...##";
            var field=new AsteroidField(map.Split(','));

            var (cell,c) = field.GetMaxVisibilityCell();

            console.WriteLine($"{cell.X} {cell.Y} => {c}");

            Assert.Equal(3,cell.X);
            Assert.Equal(4,cell.Y);
            Assert.Equal(8,c);
        }
    }
}
