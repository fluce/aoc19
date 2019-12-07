using System;
using Xunit;
using Day4;

namespace Day4.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(111111,true)]
        [InlineData(223450,false)]
        [InlineData(123789,false)]
        [InlineData(122789,true)]
        public void Test1(int input, bool result)
        {
            var impl=new Day4Impl(000000,999999);
            Assert.Equal(result,impl.Check(input));
        }
        [Theory]
        [InlineData(112233,true)]
        [InlineData(123444,false)]
        [InlineData(111122,true)]
        public void Test2(int input, bool result)
        {
            var impl=new Day4Impl(000000,999999);
            Assert.Equal(result,impl.Check2(input));
        }

    }
}
