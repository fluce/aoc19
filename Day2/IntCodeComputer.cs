using System;
using System.Linq;

namespace Day2
{

    public class IntCodeComputer
    {

        public IntCodeComputer(string input, int? state=null)
        {
            Input = input;
            buffer=input.Split(',').Select(int.Parse).ToArray();
            if (state.HasValue) { 
                buffer[1]=state.Value/100; 
                buffer[2]=state.Value%100;
            }
        }

        public string Input { get; }

        private int[] buffer;

        public Action<string> Watch {get;set;}

        private int NextPointer { get; set;}

        public void Execute()
        {
            NextPointer=0;
            var theEnd=false;
            while(!theEnd)
            {
                Watch?.Invoke(DebugOutput);
                var opcode=buffer[NextPointer++];
                switch (opcode)
                {
                    case 99: theEnd=true; break;

                    case 1: {
                        var operand1=buffer[NextPointer++];
                        var operand2=buffer[NextPointer++];
                        var resultLocation=buffer[NextPointer++];
                        buffer[resultLocation]=buffer[operand1]+buffer[operand2];
                        break;
                    }

                    case 2: {
                        var operand1=buffer[NextPointer++];
                        var operand2=buffer[NextPointer++];
                        var resultLocation=buffer[NextPointer++];
                        buffer[resultLocation]=buffer[operand1]*buffer[operand2];
                        break;
                    }

                    default: throw new Exception($"Invalid opcode : {opcode}");
                }

            }
        }

        public string Output => string.Join(",",buffer.Select(x=>x.ToString())); 
        public string DebugOutput => string.Join(",",buffer.Zip(Enumerable.Range(0,buffer.Length)).Select(x=> (x.Second==NextPointer?" ^ ":"")+x.First.ToString())); 

        public int Result => buffer[0];

        public int State => buffer[1]*100+buffer[2];

    }



}