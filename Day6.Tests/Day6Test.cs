using System;
using Xunit;

namespace Day6.Tests
{
    public class Day6Test
    {
        [Theory]
        [InlineData("COM)A","COM","A")]
        [InlineData("A)B","A","B")]
        public void ParseLine(string input,string p1, string p2)
        {
            var node=InputParser.ParseOne(input);
            Assert.Equal(p2,node.Name);
            Assert.NotNull(node.ParentNode);
            Assert.Equal(p1,node.ParentNode.Name);
            Assert.Null(node.ParentNode.ParentNode);
        }
    }
}
