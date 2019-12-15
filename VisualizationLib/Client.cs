using Grpc.Net.Client;
using IntCode;
using System;
using System.Linq;

namespace VisualizationLib
{
    public class Client
    {
        GrpcChannel Channel { get; set; }

        public Client(string url)
        {
            Channel = GrpcChannel.ForAddress(url);       
        }

        public Visualization.VisualizationClient Visualization => new Visualization.VisualizationClient(Channel);

        public IDebuggerLink DebuggerLink(string label) => new DebuggerLinkImpl(Visualization, label);

        private class DebuggerLinkImpl : IDebuggerLink
        {
            public DebuggerLinkImpl(Visualization.VisualizationClient client, string label)
            {
                Client = client;
                Label = label;
            }

            public Guid SessionId { get; } = Guid.NewGuid();
            public string Label { get; }
            public Visualization.VisualizationClient Client { get; }

            int index = 0;

            public void TraceStep(IDebuggerLink.StepData data)
            {
                Client.SendIntCodeTrace(new IntCodeTrace { 
                    Session=new UUID { Value = Google.Protobuf.ByteString.CopyFrom(SessionId.ToByteArray()) },
                    Label=Label,
                    Index=++index, 
                    InstructionPointer=data.InstructionPointer, 
                    OpCode=(int)data.OpCode,
                    BaseAddress=data.BaseAddress,
                    Instruction = Enum.GetName(typeof(IntCodeComputer.OpCode), data.OpCode),
                    Operands= { data.Operands.Select(x => new IntCodeTrace.Types.Operand { Type=(OperandType)x.Type, Modifier= (OperandModifier)x.Modifier, Value=x.Value, EffectiveValue=x.EffectiveValue }) }
                });
            }
        }

    }
}
