using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Day5;
using System.Threading.Tasks;

namespace Day7
{

    public class Amplifier
    {
        public Amplifier(string program)
        {
            Computer=new IntCodeComputer(program);
        }

        public IntCodeComputer Computer {get;private set;}

        public BlockingCollection<int> InputQueue {get;private set;} = new BlockingCollection<int>(new ConcurrentQueue<int>());

        public int? Run(int phaseSetting, int inputSignal)
        {
            Computer.Reset();
            Computer.GetNextInput = IntCodeComputer.GetInputsFrom(phaseSetting,inputSignal);
            int? res=null;
            Computer.SetOutput = x=>res=x;

            Computer.Execute();

            return res;
        }

        public Task Launch(int phaseSetting, Action<int?> outputAction)
        {
            try {
                Computer.Reset();
                Computer.GetNextInput = ()=>{ 
                    Computer.Log?.Invoke("Waiting Input"); 
                    if (InputQueue.TryTake(out var inp, 10000)) 
                        return inp;
                    throw new Exception("No more input !");
                };
                InputQueue.Add(phaseSetting);

                Computer.SetOutput = outputAction;
                
                var inputQueue=InputQueue;
                var computer=Computer;
                return Task.Factory.StartNew(()=>{ try { computer.Execute();} catch (Exception e) { computer.Log?.Invoke($"Execute error : {e.Message}"); computer.SetOutput?.Invoke(null); } } );
            } 
            catch (Exception e)
            {
                Computer.Log?.Invoke($"Launch error : {e.Message}");    
                throw;
            }
        }

    }

    public interface IAmplifiersSerie
    {
        int? Run(IEnumerable<int> sequence);  
        Action<string> Log {get;set;}
    }

    public class AmplifiersSerie:IAmplifiersSerie
    {
        public AmplifiersSerie(string program, int count)
        {
            Amplifiers=Enumerable.Range(0,count).Select(x=>new Amplifier(program)).ToArray();
        }

        public Amplifier[] Amplifiers { get; private set;}

        public Action<string> Log {get;set;}

        public int? Run(IEnumerable<int> sequence)
        {
            var input=0;
            foreach(var (amplifier,phase) in Amplifiers.Zip(sequence))
            {
                var output=amplifier.Run(phase,input);
                Log?.Invoke($"{input} phase {phase} => {output}");
                input=output.Value;
            }
            return input;
        }        
    }

    public class AmplifiersSerieWithFeedback:IAmplifiersSerie
    {
        public AmplifiersSerieWithFeedback(string program, int count)
        {
            Amplifiers=new LinkedList<Amplifier>(Enumerable.Range(0,count).Select(x=>new Amplifier(program)));
        }

        public LinkedList<Amplifier> Amplifiers { get; private set;}

        public Action<string> Log {get;set;}

        public int? Run(IEnumerable<int> sequence)
        {
            var node=Amplifiers.First;
            var tasks=new List<Task>();
            int i=0;
            int? finaloutput=0;
            foreach(var phase in sequence)
            {
                var _n=node;
                var _i=i;
                _n.Value.Computer.Log = (x)=>Log?.Invoke($"[{_i}] => {x}");
                tasks.Add(_n.Value.Launch(phase,output=>{
                    Log?.Invoke($"[{_i}] => {output}");
                    if (_n.Next==null) {
                        if (output==null)
                            Amplifiers.First.Value.InputQueue.CompleteAdding();
                        else
                            Amplifiers.First.Value.InputQueue.Add(output.Value);
                        finaloutput=output;
                    }
                    else {
                        if (output==null)
                            _n.Next.Value.InputQueue.CompleteAdding();
                        else
                            _n.Next.Value.InputQueue.Add(output.Value);
                    }
                }));
                node=node.Next;
                i++;
            }
            Amplifiers.First.Value.InputQueue.Add(0);
            Task.WaitAll(tasks.ToArray());
            return finaloutput;
        }        
    }

    public static class Helper
    {
        public static IEnumerable<int[]> GetPermutations(params int[] input)
        {
            if (input.Length==1) yield return input;
            for(int i=0;i<input.Length;i++)
            {
                var (selectedAsFirst, remain)=SplitArray(input,i);
                foreach (var p in GetPermutations(remain))
                {
                    yield return selectedAsFirst.Concat(p).ToArray();
                }
            }
        }

        public static (int[] selectedAsFirst, int[] remain) SplitArray(int[] input, int i)
        {
            var selectedAsFirst = input[i..(i+1)];
            int[] remain;
            if (i == 0)
                remain = input[(i + 1)..];
            else if (i == input.Length - 1)
                remain = input[..i];
            else
                remain = input[..i].Concat(input[(i + 1)..]).ToArray();
            return (selectedAsFirst,remain);
        }

        public static string ToPrettyString(this int[] input)
        {
            return string.Join(",",input.Select(x=>x.ToString()));
        }


        public static (int max, int[] maxPermutation) TestAllPermutation(Func<IAmplifiersSerie> amplifiersSerieFactory, params int[] phases)
        {
            int max=0;
            int[] maxPermutation=null;

            foreach(var permutation in Helper.GetPermutations(phases))
            {
                var amplifiersSerie=amplifiersSerieFactory();
                //amplifiersSerie.Log=Console.WriteLine;
                Console.WriteLine($"Trying {permutation.ToPrettyString()}");
                var result=amplifiersSerie.Run(permutation);
                if (result!=null && result>max) { max=result.Value; maxPermutation=permutation; }
                Console.WriteLine($"{permutation.ToPrettyString()} => {result} {(result==max?"MAX":"")}");
            }
            return (max,maxPermutation);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            var program="3,8,1001,8,10,8,105,1,0,0,21,46,67,76,101,118,199,280,361,442,99999,3,9,1002,9,4,9,1001,9,2,9,102,3,9,9,101,3,9,9,102,2,9,9,4,9,99,3,9,1001,9,3,9,102,2,9,9,1001,9,2,9,1002,9,3,9,4,9,99,3,9,101,3,9,9,4,9,99,3,9,1001,9,2,9,1002,9,5,9,101,5,9,9,1002,9,4,9,101,5,9,9,4,9,99,3,9,102,2,9,9,1001,9,5,9,102,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,2,9,4,9,99,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,99";
            //var program="3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5";

            /*var (max, maxPermutation) = Helper.TestAllPermutation(()=>new AmplifiersSerie(program,5),0,1,2,3,4);
            Console.WriteLine($"AmplifiersSerie : Maximum thrust is {max} for {maxPermutation.ToPrettyString()}");*/

            var (max, maxPermutation) = Helper.TestAllPermutation(()=>new AmplifiersSerieWithFeedback(program,5),5,6,7,8,9);
            Console.WriteLine($"AmplifiersSerieWithFeedback : Maximum thrust is {max} for {maxPermutation.ToPrettyString()}");
        }
    }
}
