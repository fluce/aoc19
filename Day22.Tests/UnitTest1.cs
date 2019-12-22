using System;
using Xunit;

namespace Day22.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(@"deal with increment 7
deal into new stack
deal into new stack","0 3 6 9 2 5 8 1 4 7")]
        [InlineData(@"cut 6
deal with increment 7
deal into new stack","3 0 7 4 1 8 5 2 9 6")]
        [InlineData(@"deal with increment 7
deal with increment 9
cut -2","6 3 0 7 4 1 8 5 2 9")]
        [InlineData(@"deal into new stack
cut -2
deal with increment 7
cut 8
cut -4
deal with increment 7
cut 3
deal with increment 9
deal with increment 3
cut -1","9 2 5 8 1 4 7 0 3 6")]
        public void Test(string program,string result)
        {
            var deck=new Deck(10);
            deck.Play(program.Split(new char[] { '\r', '\n' }));
            Assert.Equal(result,string.Join(" ",deck.GetData()));
        }

        [Theory]
        [InlineData(@"deal with increment 7
deal into new stack
deal into new stack","0 3 6 9 2 5 8 1 4 7")]
        [InlineData(@"cut 6
deal with increment 7
deal into new stack","3 0 7 4 1 8 5 2 9 6")]
        [InlineData(@"deal with increment 7
deal with increment 9
cut -2","6 3 0 7 4 1 8 5 2 9")]
        [InlineData(@"deal into new stack
cut -2
deal with increment 7
cut 8
cut -4
deal with increment 7
cut 3
deal with increment 9
deal with increment 3
cut -1","9 2 5 8 1 4 7 0 3 6")]
        public void TestLineao(string program,string result)
        {
            var deck=new Deck(10);
            deck.OptimalImplementation=true;
            deck.Play(program.Split(new char[] { '\r', '\n' }));
            Assert.Equal(result,string.Join(" ",deck.GetData()));
        }

    }
}
