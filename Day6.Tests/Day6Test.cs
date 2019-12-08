using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Day6.Tests
{
    public class Day6Test
    {
        readonly ITestOutputHelper console;

        public Day6Test(ITestOutputHelper console)
        {
            this.console = console;
        }

        [Theory]
        [InlineData("COM)A","COM","A")]
        [InlineData("A)B","A","B")]
        public void TestParseLine(string input,string p1, string p2)
        {
            var node=InputParser.ParseOne(input);
            Assert.Equal(p2,node.Name);
            Assert.NotNull(node.OrbitsAround);
            Assert.Equal(p1,node.OrbitsAround.Name);
            Assert.Null(node.OrbitsAround.OrbitsAround);
            Assert.Single(node.OrbitsAround.Satellites);
            Assert.Same(node,node.OrbitsAround.Satellites.Single());
            Assert.Empty(node.Satellites);            
        }

        [Theory]
        [InlineData(@"COM)B,B)C,C)D,D)E,E)F,B)G,G)H,D)I,E)J,J)K,K)L")]
        [InlineData(@"COM)B,C)D,B)C")]
        public void TestParser(string input)
        {
            var parser=new InputParser(input.Split(","));
            parser.Parse();
            Assert.Single(parser.Roots);
            InputParser.Dump(parser.Roots.First(),console.WriteLine);
        }

        [Fact]
        public void TestCalculateOrbits()
        {
            string input=@"COM)B,B)C,C)D,D)E,E)F,B)G,G)H,D)I,E)J,J)K,K)L";
            var parser=new InputParser(input.Split(","));
            parser.Parse();
            Assert.Single(parser.Roots);
            InputParser.Dump(parser.Roots.First(),console.WriteLine);

            Assert.Equal(3,InputParser.CalculateDirectAndIndirectOrbits(parser.AllObjects["D"]));
            Assert.Equal(7,InputParser.CalculateDirectAndIndirectOrbits(parser.AllObjects["L"]));
            Assert.Equal(0,InputParser.CalculateDirectAndIndirectOrbits(parser.AllObjects["COM"]));

            var result=parser.AllObjects.Values.Sum(InputParser.CalculateDirectAndIndirectOrbits);
            Assert.Equal(42,result);
        }

        [Theory]
        [InlineData(@"COM)B,B)C,C)D,D)E,E)F,B)G,G)H,D)I,E)J,J)K,K)L", "C", "B,COM")]
        [InlineData(@"COM)B,B)C,C)D,D)E,E)F,B)G,G)H,D)I,E)J,J)K,K)L", "L", "K,J,E,D,C,B,COM")]
        [InlineData(@"COM)B,B)C,C)D,D)E,E)F,B)G,G)H,D)I,E)J,J)K,K)L", "H", "G,B,COM")]
        public void TestGetPathToRoot(string input, string from, string expectedResult)
        {
            var parser=new InputParser(input.Split(","));
            parser.Parse();
            InputParser.Dump(parser.Roots.First(),console.WriteLine);
            var r=string.Join(",",InputParser.GetPathToRootSpatialObject(parser.AllObjects[from]).Select(x=>x.Name));
            Assert.Equal(expectedResult,r);
        }

        [Theory]
        [InlineData(@"COM)B,B)C,C)D,D)E,E)F,B)G,G)H,D)I,E)J,J)K,K)L", "B", "COM", 1)]
        [InlineData(@"COM)B,B)C,C)D,D)E,E)F,B)G,G)H,D)I,E)J,J)K,K)L", "C", "COM", 2)]
        [InlineData(@"COM)B,B)C,C)D,D)E,E)F,B)G,G)H,D)I,E)J,J)K,K)L", "C", "G", 0)]
        [InlineData(@"COM)B,B)C,C)D,D)E,E)F,B)G,G)H,D)I,E)J,J)K,K)L,K)YOU,I)SAN", "YOU", "SAN", 4)]
        public void TestCalcTransferCount(string input, string from, string to, int expectedResult)
        {
            var parser=new InputParser(input.Split(","));
            parser.Parse();
            InputParser.Dump(parser.Roots.First(),console.WriteLine);

            console.WriteLine(string.Join(",",InputParser.GetPathToRootSpatialObject(parser.AllObjects[from]).Select(x=>x.Name)));
            console.WriteLine(string.Join(",",InputParser.GetPathToRootSpatialObject(parser.AllObjects[to]).Select(x=>x.Name)));

            var (degenerative,n)=parser.CalculateOrbitalTransfersCount(parser.AllObjects[from],parser.AllObjects[to], console.WriteLine);
            if (degenerative) console.WriteLine("Degenerative case");
            Assert.Equal(expectedResult,n);
        }

    }
}
