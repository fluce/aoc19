using System;
using System.Collections.Generic;
using System.Linq;

namespace IntCode
{

    public class Snapshot
    {
        public long[] Buffer;
        public Dictionary<long,long> AdditionnalMemory;
        public long NextPointer;
        public long BaseAddress;
    }

    public class IntCodeComputer
    {

        public IntCodeComputer(string program, long? state=null)
        {
            Program = program;
            Memory=new MemoryAccessor(this);
            Reset(state);
        }

        public void Reset(long? state=null)
        {
            buffer=Program.Split(',').Select(long.Parse).ToArray();
            if (state.HasValue) { 
                buffer[1]=state.Value/100; 
                buffer[2]=state.Value%100;
            }
            NextPointer=0;
            additionnalMemory.Clear();
        }

        public string Program { get; }

        private long[] buffer;
        private Dictionary<long,long> additionnalMemory=new Dictionary<long, long>();

        public Action<string> Watch {get;set;}
        public Action<string> Log {get;set;}

        public Func<long> GetNextInput { get; set;}
        public Action<long?> SetOutput {get;set;}

        private long NextPointer { get; set; }
        private long BaseAddress { get; set; }

        enum OpCode {
            ADD=1,
            MUL=2,
            INP=3,
            OUT=4,
            JNZ=5,
            JZE=6,
            TLT=7,
            TEQ=8,
            ADB=9,
            END=99,
        }

        public void Execute(Snapshot snapshot=null)
        {
            if (snapshot==null) {
                NextPointer=0;
                BaseAddress=0;
                additionnalMemory.Clear();
            } else {
                buffer=snapshot.Buffer.ToArray();
                additionnalMemory=snapshot.AdditionnalMemory.ToDictionary(x=>x.Key, x=>x.Value);
                BaseAddress=snapshot.BaseAddress;
                NextPointer=snapshot.NextPointer;
            }

            var theEnd=false;
            while(!theEnd)
            {
                Watch?.Invoke(DebugOutput);
                var instructionPointer=NextPointer;
                var opcode=(int)Memory[NextPointer++];

                var instruction=(OpCode)(opcode%100);
                switch (instruction)
                {
                    case OpCode.END: theEnd=true; Log?.Invoke($"{instructionPointer,6} 99 END"); break;

                    case OpCode.ADD: {
                        NextPointer=RunInstruction_2_1(opcode,Memory,instructionPointer,NextPointer,(b1,b2)=>b1+b2);
                        break;
                    }

                    case OpCode.MUL: {
                        NextPointer=RunInstruction_2_1(opcode,Memory,instructionPointer,NextPointer,(b1,b2)=>b1*b2);
                        break;
                    }

                    case OpCode.TEQ: {
                        NextPointer=RunInstruction_2_1(opcode,Memory,instructionPointer,NextPointer,(b1,b2)=>b1==b2?1:0);
                        break;
                    }

                    case OpCode.TLT: {
                        NextPointer=RunInstruction_2_1(opcode,Memory,instructionPointer,NextPointer,(b1,b2)=>b1<b2?1:0);
                        break;
                    }

                    case OpCode.JNZ: {
                        NextPointer=RunInstruction_2_J(opcode,Memory,instructionPointer,NextPointer,(b1)=>b1!=0);
                        break;
                    }

                    case OpCode.JZE: {
                        NextPointer=RunInstruction_2_J(opcode,Memory,instructionPointer,NextPointer,(b1)=>b1==0);
                        break;
                    }

                    case OpCode.INP: {
                        NextPointer=RunInstruction_0_1(opcode,Memory,instructionPointer,NextPointer,GetNextInput);
                        break;
                    }

                    case OpCode.OUT: {
                        NextPointer=RunInstruction_1_0(opcode,Memory,instructionPointer,NextPointer,x=>SetOutput?.Invoke(x));
                        break;
                    }

                    case OpCode.ADB: {
                        NextPointer=RunInstruction_1_0(opcode,Memory,instructionPointer,NextPointer,x=>BaseAddress+=x);
                        break;
                    }

                    default: throw new Exception($"Invalid opcode : {opcode}");
                }

            }
        }

        public MemoryAccessor Memory { get; private set; }

        public class MemoryAccessor
        {
            internal MemoryAccessor(IntCodeComputer computer)
            {
                this.computer=computer;
            }

            private IntCodeComputer computer;

            public long this[long address] 
            { 
                get {
                    if (address>=0 && address < computer.buffer.Length)
                        return computer.buffer[address];
                    else if (computer.additionnalMemory.TryGetValue(address,out var value))
                        return value;
                    return 0;
                } 
                set { 
                    if (address>=0 && address <computer.buffer.Length) {
                        computer.buffer[address]=value;
                    }
                    else {
                        computer.additionnalMemory[address]=value;
                    }
                }
            }
        }

        public long HandleOperand(MemoryAccessor buf, long operand, int mod)
        {
            return mod switch {
                1=>operand,
                2=>buf[BaseAddress+operand],
                _=>buf[operand]
            };
        }

        private long RunInstruction_2_1(int opcode, MemoryAccessor buf, long instructionPointer, long nextPointer, Func<long,long,long> operation)
        {
            var modifiers=opcode/100;
            opcode%=100;

            var mod1=modifiers%10; modifiers/=10;
            var mod2=modifiers%10; modifiers/=10;
            var modR=modifiers%10; modifiers/=10;

            var operand1=buf[nextPointer++];
            var operand2=buf[nextPointer++];
            var resultLocation=buf[nextPointer++];
            var b1=HandleOperand(buf, operand1, mod1);
            var b2=HandleOperand(buf, operand2, mod2);
            var bR=resultLocation+(modR==2?BaseAddress:0);
            var r=operation(b1,b2);
            buf[bR]=r;
            Log?.Invoke($"{instructionPointer,6} {(int)opcode,2:00} {Enum.GetName(typeof(OpCode),opcode)} {FormatOperand(mod1,operand1)} {FormatOperand(mod2,operand2)} => [{resultLocation}]  ({b1} {b2} => {r})");
            return nextPointer;
        }

        private long RunInstruction_0_1(int opcode, MemoryAccessor buf, long instructionPointer, long nextPointer, Func<long> operation)
        {
            var modifiers=opcode/100;
            opcode%=100;

            var modR=modifiers%10; modifiers/=10;
            var resultLocation=buf[nextPointer++];
            var bR=resultLocation+(modR==2?BaseAddress:0);

            var r=operation();

            buf[bR]=r;

            Log?.Invoke($"{instructionPointer,6} {(int)opcode,2:00} {Enum.GetName(typeof(OpCode),opcode)} => [{resultLocation}]  ( => {r})");
            return nextPointer;
        }

        private long RunInstruction_1_0(int opcode, MemoryAccessor buf, long instructionPointer, long nextPointer, Action<long> operation)
        {
            var modifiers=opcode/100;
            opcode%=100;
            var mod1=modifiers%10; modifiers/=10;
            var operand1=buf[nextPointer++];
            var b1=HandleOperand(buf, operand1, mod1);
            operation(b1);

            Log?.Invoke($"{instructionPointer,6} {(int)opcode,2:00} {Enum.GetName(typeof(OpCode),opcode)} {FormatOperand(mod1,operand1)} =>  ({b1} => )");
            return nextPointer;
        }

        private long RunInstruction_2_J(int opcode, MemoryAccessor buf, long instructionPointer, long nextPointer, Func<long,bool> test)
        {
            var modifiers=opcode/100;
            opcode%=100;
            var mod1=modifiers%10; modifiers/=10;
            var mod2=modifiers%10; modifiers/=10;

            var operand1=buf[nextPointer++];
            var operand2=buf[nextPointer++];
            var b1=HandleOperand(buf, operand1, mod1);
            var b2=HandleOperand(buf, operand2, mod2);

            var match=test(b1);
            if (match) 
                nextPointer=b2;

            Log?.Invoke($"{instructionPointer,6} {(int)opcode,2:00} {Enum.GetName(typeof(OpCode),opcode)} {FormatOperand(mod1,operand1)} {FormatOperand(mod2,operand2)} =>  ({b1} => {(match?"MATCHED ":"")}nextPointer={nextPointer})");
            return nextPointer;
        }

        private string FormatOperand(int modifier, long operand)=>modifier switch { 1=>operand.ToString(), 2=>$"[{BaseAddress}+{operand}]", _=>$"[{operand}]" };

        public string Output => string.Join(",",buffer.Select(x=>x.ToString())); 
        public string DebugOutput => string.Join(",",buffer.Zip(Enumerable.Range(0,buffer.Length),(First,Second)=>(First,Second)).Select(x=> (x.Second==NextPointer?" ^ ":"")+x.First.ToString())); 

        public long Result => Memory[0];

        public long State => Memory[1]*100+Memory[2];

        public Snapshot TakeSnapshot()
        {
            return new Snapshot() {
                Buffer=buffer.ToArray(),
                AdditionnalMemory=additionnalMemory.ToDictionary(x=>x.Key, x=>x.Value),
                BaseAddress=BaseAddress,
                NextPointer=NextPointer
            };
        }

        public static Func<long> GetInputsFrom(params long[] inputs)=>GetInputsFrom(inputs as IEnumerable<long>);

        public static Func<long> GetInputsFrom(IEnumerable<long> inputs)
        {
            var enumerator=inputs.GetEnumerator();
            return ()=>{ enumerator.MoveNext(); return enumerator.Current; };
        }


    }



}