using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace VisualizationLib.Tests
{
    public class VisualizationLibTest
    {
        readonly ITestOutputHelper console;

        public VisualizationLibTest(ITestOutputHelper console)
        {
            this.console = console;
            Environment.SetEnvironmentVariable("http_proxy", "http://localhost:12345");
        }


        [Fact]
        public void TestClient()
        {
            var client = new Client("https://localhost:5001");
            var cli = client.Visualization;
            cli.SendIntCodeTrace(new IntCodeTrace { Index = 1, InstructionPointer = 2, OpCode = 101, Instruction = "ADD", Operands = { new IntCodeTrace.Types.Operand { Type = OperandType.Input, Modifier = OperandModifier.Direct, Value = 5, EffectiveValue = 5 } } });
        }

    }
}