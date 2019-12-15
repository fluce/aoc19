namespace Day9.Tests
{
    public class IntCodeComputerDay5Test
    {
        private readonly ITestOutputHelper console;

        public IntCodeComputerDay5Test(ITestOutputHelper console)
        {
            this.console = console;
        }

        [Theory]
        [InlineData("1001,4,3,4,96","1001,4,3,4,99")]
        [InlineData("1001,4,-3,4,102","1001,4,-3,4,99")]
        [InlineData("1002,4,3,4,33","1002,4,3,4,99")]
        [InlineData("0101,3,4,4,96","101,3,4,4,99")]
        [InlineData("0102,3,4,4,33","102,3,4,4,99")]
        public void TestIndirect(string program, string expected_result)
        {
            var impl=new IntCodeComputer(program);
            impl.Log=(x)=>console.WriteLine(x);
            impl.Execute();
            Assert.Equal(expected_result, impl.Output);
        }

        [Theory]

        [InlineData("3,0,4,0,99",5,5)]

        [InlineData("3,9,8,9,10,9,4,9,99,-1,8",8,1)]
        [InlineData("3,9,8,9,10,9,4,9,99,-1,8",5,0)]
        [InlineData("3,9,7,9,10,9,4,9,99,-1,8",7,1)]
        [InlineData("3,9,7,9,10,9,4,9,99,-1,8",8,0)]
        [InlineData("3,9,7,9,10,9,4,9,99,-1,8",9,0)]

        [InlineData("3,3,1108,-1,8,3,4,3,99",8,1)]
        [InlineData("3,3,1108,-1,8,3,4,3,99",5,0)]
        [InlineData("3,3,1107,-1,8,3,4,3,99",7,1)]
        [InlineData("3,3,1107,-1,8,3,4,3,99",8,0)]
        [InlineData("3,3,1107,-1,8,3,4,3,99",9,0)]

        [InlineData("3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9",0,0)]
        [InlineData("3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9",1,1)]
        [InlineData("3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9",5,1)]

        [InlineData("3,3,1105,-1,9,1101,0,0,12,4,12,99,1",0,0)]
        [InlineData("3,3,1105,-1,9,1101,0,0,12,4,12,99,1",1,1)]
        [InlineData("3,3,1105,-1,9,1101,0,0,12,4,12,99,1",5,1)]

        [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99",-30,999)]
        [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99",7,999)]
        [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99",8,1000)]
        [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99",9,1001)]
        [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99",50,1001)]

        public void TestInputOutput(string program, int input, int expected_output)
        {
            var impl=new IntCodeComputer(program);
            impl.GetNextInput=()=>input;
            impl.Log=(x)=>console.WriteLine(x);
            long? output=null;
            impl.SetOutput=x=>output=x;
            impl.Execute();
            Assert.Equal(expected_output, output);
        }
    }
}
