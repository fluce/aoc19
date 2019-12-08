using System;
using System.Collections.Generic;
using System.Linq;
using Day5;

namespace Day7
{

    public class Amplifier
    {
        public Amplifier(string program)
        {
            Computer=new IntCodeComputer(program);
        }

        public IntCodeComputer Computer {get;private set;}

        public int? Run(int phaseSetting, int inputSignal)
        {
            Computer.Reset();
            Computer.GetNextInput = IntCodeComputer.GetInputsFrom(phaseSetting, inputSignal);
            int? res=null;
            Computer.SetOutput = x=>res=x;

            Computer.Execute();

            return res;
        }

    }

    public class AmplifiersSerie
    {
        public AmplifiersSerie(string program, int count)
        {
            Amplifiers=Enumerable.Range(0,count).Select(x=>new Amplifier(program)).ToArray();
        }

        public Amplifier[] Amplifiers { get; private set;}

        public Action<string> Log {get;set;}

        public int Run(IEnumerable<int> sequence)
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

    }

    class Program
    {
        static void Main(string[] args)
        {
            var program="3,8,1001,8,10,8,105,1,0,0,21,46,67,76,101,118,199,280,361,442,99999,3,9,1002,9,4,9,1001,9,2,9,102,3,9,9,101,3,9,9,102,2,9,9,4,9,99,3,9,1001,9,3,9,102,2,9,9,1001,9,2,9,1002,9,3,9,4,9,99,3,9,101,3,9,9,4,9,99,3,9,1001,9,2,9,1002,9,5,9,101,5,9,9,1002,9,4,9,101,5,9,9,4,9,99,3,9,102,2,9,9,1001,9,5,9,102,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,2,9,4,9,99,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,99";
            //var program="3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0";
            var amp=new AmplifiersSerie(program,5);
            
            //foreach(var a in amp.Amplifiers) a.Computer.Log = Console.WriteLine;
            //amp.Log=Console.WriteLine;

            int max=0;
            int[] maxPermutation=null;
            foreach(var permutation in Helper.GetPermutations(0,1,2,3,4))
            {
                var result=amp.Run(permutation);
                if (result>max) { max=result; maxPermutation=permutation; }
                Console.WriteLine($"{permutation.ToPrettyString()} => {result} {(result==max?"MAX":"")}");
            }

            Console.WriteLine($"Maximum thrust is {max} for {maxPermutation.ToPrettyString()}");
        }
    }
}
