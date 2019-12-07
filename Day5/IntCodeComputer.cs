using System;
using System.Linq;

namespace Day5
{

    public class IntCodeComputer
    {

        public IntCodeComputer(string program, int? state=null)
        {
            Program = program;
            buffer=program.Split(',').Select(int.Parse).ToArray();
            if (state.HasValue) { 
                buffer[1]=state.Value/100; 
                buffer[2]=state.Value%100;
            }
        }

        public string Program { get; }

        private int[] buffer;

        public Action<string> Watch {get;set;}
        public Action<string> Log {get;set;}

        public Func<int> GetNextInput { get; set;}
        public Action<int> SetOutput {get;set;}

        private int NextPointer { get; set;}

        enum OpCode {
            ADD=1,
            MUL=2,
            INP=3,
            OUT=4,
            JNZ=5,
            JZE=6,
            TLT=7,
            TEQ=8,
            END=99,
        }

        public void Execute()
        {
            NextPointer=0;
            var theEnd=false;
            while(!theEnd)
            {
                Watch?.Invoke(DebugOutput);
                var instructionPointer=NextPointer;
                var opcode=buffer[NextPointer++];

                var instruction=(OpCode)(opcode%100);
                switch (instruction)
                {
                    case OpCode.END: theEnd=true; Log?.Invoke($"{instructionPointer,6} 99 END"); break;

                    case OpCode.ADD: {
                        NextPointer=RunInstruction_2_1(opcode,buffer,instructionPointer,NextPointer,(b1,b2)=>b1+b2);
                        break;
                    }

                    case OpCode.MUL: {
                        NextPointer=RunInstruction_2_1(opcode,buffer,instructionPointer,NextPointer,(b1,b2)=>b1*b2);
                        break;
                    }

                    case OpCode.TEQ: {
                        NextPointer=RunInstruction_2_1(opcode,buffer,instructionPointer,NextPointer,(b1,b2)=>b1==b2?1:0);
                        break;
                    }

                    case OpCode.TLT: {
                        NextPointer=RunInstruction_2_1(opcode,buffer,instructionPointer,NextPointer,(b1,b2)=>b1<b2?1:0);
                        break;
                    }

                    case OpCode.JNZ: {
                        NextPointer=RunInstruction_2_J(opcode,buffer,instructionPointer,NextPointer,(b1)=>b1!=0);
                        break;
                    }

                    case OpCode.JZE: {
                        NextPointer=RunInstruction_2_J(opcode,buffer,instructionPointer,NextPointer,(b1)=>b1==0);
                        break;
                    }

                    case OpCode.INP: {
                        NextPointer=RunInstruction_0_1(opcode,buffer,instructionPointer,NextPointer,GetNextInput);
                        break;
                    }

                    case OpCode.OUT: {
                        NextPointer=RunInstruction_1_0(opcode,buffer,instructionPointer,NextPointer,SetOutput);
                        break;
                    }

                    default: throw new Exception($"Invalid opcode : {opcode}");
                }

            }
        }

        private int RunInstruction_2_1(int opcode, int[] buf, int instructionPointer, int nextPointer, Func<int,int,int> operation)
        {
            var modifiers=opcode/100;
            opcode%=100;

            var mod1=modifiers%10; modifiers/=10;
            var mod2=modifiers%10; modifiers/=10;

            var operand1=buf[nextPointer++];
            var operand2=buf[nextPointer++];
            var resultLocation=buf[nextPointer++];
            var b1=mod1==1?operand1:buf[operand1];
            var b2=mod2==1?operand2:buf[operand2];
            var r=operation(b1,b2);
            buffer[resultLocation]=r;
            Log?.Invoke($"{instructionPointer,6} {(int)opcode,2:00} {Enum.GetName(typeof(OpCode),opcode)} {FormatOperand(mod1,operand1)} {FormatOperand(mod2,operand2)} => [{resultLocation}]  ({b1} {b2} => {r})");
            return nextPointer;
        }

        private int RunInstruction_0_1(int opcode, int[] buf, int instructionPointer, int nextPointer, Func<int> operation)
        {
            var resultLocation=buf[nextPointer++];
            var r=operation();
            buffer[resultLocation]=r;

            Log?.Invoke($"{instructionPointer,6} {(int)opcode,2:00} {Enum.GetName(typeof(OpCode),opcode)} => [{resultLocation}]  ( => {r})");
            return nextPointer;
        }

        private int RunInstruction_1_0(int opcode, int[] buf, int instructionPointer, int nextPointer, Action<int> operation)
        {
            var modifiers=opcode/100;
            opcode%=100;
            var mod1=modifiers%10; modifiers/=10;

            var operand1=buf[nextPointer++];
            var b1=mod1==1?operand1:buf[operand1];
            operation(b1);

            Log?.Invoke($"{instructionPointer,6} {(int)opcode,2:00} {Enum.GetName(typeof(OpCode),opcode)} {FormatOperand(mod1,operand1)} =>  ({b1} => )");
            return nextPointer;
        }

        private int RunInstruction_2_J(int opcode, int[] buf, int instructionPointer, int nextPointer, Func<int,bool> test)
        {
            var modifiers=opcode/100;
            opcode%=100;
            var mod1=modifiers%10; modifiers/=10;
            var mod2=modifiers%10; modifiers/=10;

            var operand1=buf[nextPointer++];
            var operand2=buf[nextPointer++];
            var b1=mod1==1?operand1:buf[operand1];
            var b2=mod2==1?operand2:buf[operand2];

            var match=test(b1);
            if (match) 
                nextPointer=b2;

            Log?.Invoke($"{instructionPointer,6} {(int)opcode,2:00} {Enum.GetName(typeof(OpCode),opcode)} {FormatOperand(mod1,operand1)} {FormatOperand(mod2,operand2)} =>  ({b1} => {(match?"MATCHED ":"")}nextPointer={nextPointer})");
            return nextPointer;
        }

        private string FormatOperand(int modifier, int operand)=>modifier==1?operand.ToString():$"[{operand}]";

        public string Output => string.Join(",",buffer.Select(x=>x.ToString())); 
        public string DebugOutput => string.Join(",",buffer.Zip(Enumerable.Range(0,buffer.Length)).Select(x=> (x.Second==NextPointer?" ^ ":"")+x.First.ToString())); 

        public int Result => buffer[0];

        public int State => buffer[1]*100+buffer[2];

    }



}