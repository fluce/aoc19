using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Day16
{
    public class FFT
    {
        public FFT()
        {
        }

        public static Action<string> Log {get;set;}

        public string Apply(string input, int phaseCount)
        {
            return string.Join("",Apply(input.ToCharArray().Select(x=>Convert.ToByte(x)-48).Select(x=>(byte)x).ToArray(), phaseCount).Take(8));
        }

        public string ApplyFull(string input)
        {
            var inputi=input.ToCharArray().Select(x=>Convert.ToByte(x)-48);
            var allinput=Enumerable.Range(0,10000).SelectMany(x=>inputi).Select(x=>(byte)x).ToArray();
            int offset=int.Parse(input.Substring(0,7));
            return string.Join("",Apply2(allinput,100,offset).Skip(offset).Take(8));
        }

        public static IEnumerable<byte> Apply(byte[] input, int phaseCount)
        {
            var ret=input;
            var output=new byte[input.Length];
            for(int i=0;i<phaseCount;i++)
            {
                Array.Clear(output,0,output.Length);
                ApplyPhase(ret,output);
                var a=ret;
                ret=output;
                output=a;
            }
            return ret;
        }

        public static IEnumerable<byte> Apply2(byte[] input, int phaseCount, int offset)
        {
            var ret=input;
            var output=new byte[input.Length];
            Array.Copy(input,output,input.Length);
            for(int i=0;i<phaseCount;i++)
            {
                ApplyPhase2(output,offset);
            }
            return output;
        }

        public static void ApplyPhase2(byte[] data, int offset)
        {
            int total=0;
            for(int i=data.Length-1;i>=offset;i--)
            {
                total+=data[i];                
                data[i]=(byte)(total%10);
            }
        }

        public static void ApplyPhase(byte[] input, byte[] output)
        {
            Parallel.For(0,input.Length,(i,state)=> { output[i]=ApplyPattern(input, i); } );
        }

        public static IEnumerable<int> ApplyPhaseV1(IEnumerable<int> input, int[] pattern)
        {
            int i=0;
            foreach(var e in input)
                yield return ApplyPatternV1(input, CalcPattern(pattern,i++).Skip(1));
        }

        public static IEnumerable<int> CalcPattern(int[] pattern, int index)
        {            
            while (true)
                foreach(var e in pattern.SelectMany(x=>Enumerable.Range(0,index+1).Select(y=>x)))
                    yield return e;
        }

        public static byte ApplyPattern(byte[] input, int idx)
        {
            int start=idx; // idx start at 0
            int rcount=idx+1; // repeat count;
            int remain=rcount;

            int ret=0;

            int current=start;
            int state=1;
            while (current<input.Length)
            {                
                if (remain>0) {
                    if (state==1)
                        ret+=input[current];
                    else
                        ret-=input[current];
                    remain--;
                    current++;
                } else {
                    current+=rcount; // skip rcount '0'
                    state=-state;
                    remain=rcount;
                }
            }
            return (byte)(Math.Abs(ret)%10);

        }

        public static byte ApplyPatternV2(byte[] input, int idx)
        {
            int start=idx; // idx start at 0
            int rcount=idx+1; // repeat count;
            int remain=rcount;

            int ret=0;

            int current=start;
            int state=1;
            while (current<input.Length)
            {                
                if (remain>0) {
                    if (state==1)
                        ret+=input[current];
                    else
                        ret-=input[current];
                    remain--;
                    current++;
                } else {
                    current+=rcount; // skip rcount '0'
                    state=-state;
                    remain=rcount;
                }
            }
            return (byte)(Math.Abs(ret)%10);

        }
        public static int ApplyPatternV1(IEnumerable<int> input, IEnumerable<int> pattern)
        {
            var ret1=input.Zip(pattern).Sum(x=>x.First*x.Second);
            var ret=Math.Abs(ret1)%10;
            //Log?.Invoke($"{string.Join(" + ",input.Zip(pattern).Select(x=>$"{x.First} * {x.Second}"))} = {ret1} => {ret}");
            return ret;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input="59718730609456731351293131043954182702121108074562978243742884161871544398977055503320958653307507508966449714414337735187580549358362555889812919496045724040642138706110661041990885362374435198119936583163910712480088609327792784217885605021161016819501165393890652993818130542242768441596060007838133531024988331598293657823801146846652173678159937295632636340994166521987674402071483406418370292035144241585262551324299766286455164775266890428904814988362921594953203336562273760946178800473700853809323954113201123479775212494228741821718730597221148998454224256326346654873824296052279974200167736410629219931381311353792034748731880630444730593";
            var fft=new FFT();            
            var ret=fft.Apply(input,100);
            Console.WriteLine($"Result part 1= {ret}");
            var ret2=fft.ApplyFull(input);
            Console.WriteLine($"Result part 2 = {ret2}");
        }
    }
}
