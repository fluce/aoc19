using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;
using Utils;

namespace Day22
{
    public class Deck
    {
        int[] data;
        long size;
        public Deck(int size)
        {
            data=new int[size];
            this.size=size;
        }
        public IEnumerable<int> GetData() => data;
        public void FactoryOrder()
        {
            data=Enumerable.Range(0,(int)size).ToArray();
        }

        public static int[] DealIntoNewStack(int[] data, int n)
        {
            return data.Reverse().ToArray();
        }

        public static int[] CutNCard(int[] data, int n)
        {
            if (n>=0)
                return data.Skip(n).Concat(data.Take(n)).ToArray();
            else
                return data.Skip(data.Length+n).Concat(data.Take(data.Length+n)).ToArray();
        }

        public static int[] DealWithIncrement(int[] data, int n)
        {
            var dest=new int[data.Length];
            int idx=0;
            foreach(var card in data) {
                dest[idx]=card;
                idx+=n;
                idx=idx%data.Length;
            }
            return dest;
        }

        private static Func<string,int?> SimpleString(string s) { return (input)=>(s==input)?0:(int?)null; }
        private static Func<string,int?> RegularExpression(string s) { 
            var regex=new Regex(s);  
            return (input)=> { 
                var res=regex.Match(input); 
                if (res.Success) return int.Parse(res.Groups["n"].Value); 
                else return (int?)null;  
            }; 
        }

        static (Func<string,int?> parse,Func<int[],int, int[]> action, Func<BigInteger,BigInteger,(BigInteger a, BigInteger b)> factors)[] actions={ 
            ( SimpleString("deal into new stack"), DealIntoNewStack, (N,p)=>(-1,N-1) ), 
            ( RegularExpression(@"deal with increment (?<n>\d+)"), DealWithIncrement, (N,p)=>(p,0) ),
            ( RegularExpression(@"cut (?<n>-?\d+)"), CutNCard, (N,p)=>(1,N-p) ),
        };

        public bool OptimalImplementation {get;set;}

        
        public void Play(string progline)
        {
            if (string.IsNullOrWhiteSpace(progline)) return;

            var r=actions.Select(x=>(x,x.parse(progline))).First(x=>x.Item2!=null);
            
            if (OptimalImplementation) 
            {
                var factors=r.x.factors(size,r.Item2.Value);
                var data2=data.ToArray();
                for(int i=0;i<size;i++)
                {
                    var newi=i*factors.a+factors.b;
                    newi=newi%size;
                    data2[(int)newi]=data[i];
                }
                data=data2;
            }
            else 
            {
                data=r.x.action(data,r.Item2.Value);
            }
        }

        public static IEnumerable<(BigInteger a, BigInteger b)> GetFactors(BigInteger size, string[] program)
        {
            foreach(var progline in program)
            {
                if (!string.IsNullOrWhiteSpace(progline)) {
                    var r=actions.Select(x=>(x,x.parse(progline))).First(x=>x.Item2!=null);
                    yield return r.x.factors(size,r.Item2.Value);
                }
            }            
        }

        public static (BigInteger a, BigInteger b) CalcCumulatedFactors(IEnumerable<(BigInteger a, BigInteger b)> successivefactors)
        {
            (BigInteger a, BigInteger b) cumulatedfactors=(1,0);
            foreach(var factors in successivefactors)
            {   
                cumulatedfactors.a*=factors.a;
                cumulatedfactors.b*=factors.a;
                cumulatedfactors.b+=factors.b;
                //Console.WriteLine($"{factors} => {cumulatedfactors}");
            }
            return cumulatedfactors;
        }

        public static BigInteger GetCardAfterNIteration(BigInteger deskSize, BigInteger iteration, BigInteger location, string[] program)
        {
            var successivefactors=GetFactors(deskSize,program);
            var (a,b)=CalcCumulatedFactors(successivefactors);
            Console.WriteLine($"Factors={(a,b)}");
            var apown1=BigInteger.ModPow(a,iteration,deskSize*(a-1));

            BigInteger ret=0;
            ret=( location - b * (apown1-1) / (a-1) ) * BigInteger.ModPow(apown1,deskSize-2,deskSize);
            ret=((ret%deskSize)+deskSize)%deskSize;

            return ret;
        }

        public static BigInteger GetLocationAfterNIteration(BigInteger deskSize, BigInteger iteration, BigInteger card, string[] program)
        {
            var successivefactors=GetFactors(deskSize,program);
            var (a,b)=CalcCumulatedFactors(successivefactors);

            BigInteger ret=0;
            /*if (iteration==1)
                ret=card*a+b;
            else*/
            var apown=BigInteger.ModPow(a,iteration,deskSize*(a-1));
            ret=card*apown+b*(apown-1)/(a-1);
            
            ret=((ret%deskSize)+deskSize)%deskSize;

            return ret;
        }

        public void Play(string[] program)
        {
            FactoryOrder();
            if (OptimalImplementation) 
            {
                var successivefactors=GetFactors(size,program);
                var cumulatedfactors=CalcCumulatedFactors(successivefactors);
                
                var data2=data.ToArray();
                for(long i=0;i<size;i++)
                {
                    BigInteger newi=cumulatedfactors.a*i+cumulatedfactors.b;
                    /*foreach(var factors in successivefactors)
                    {
                        newi=newi*factors.a+factors.b;
                    }*/
                    newi=((newi%size)+size)%size;
                    if (newi>=size || newi<0) throw new Exception("Index out of bound : "+newi);
                    data2[(int)newi]=data[i];
                }
                data=data2;
            } 
            else
                foreach(var s in program)
                    try {
                        Play(s);
                    } catch (Exception e) {
                        throw new Exception("Error playing "+s+" : "+e.Message,e);
                    }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var deck=new Deck(10007);
            deck.OptimalImplementation=true;
            deck.Play(System.IO.File.ReadAllLines(args[0]));
            var arr=deck.GetData().ToArray();
            var location=arr.Index().Single(x=>x.Item==2019).Idx;
            Console.WriteLine($"Result part 1 : {location} {arr[location]}");
            var ret3=Deck.GetCardAfterNIteration(10007,1, location, System.IO.File.ReadAllLines(args[0]));
            Console.WriteLine($"{location}=>{ret3}");
            var ret4=Deck.GetLocationAfterNIteration(10007,1, 2019, System.IO.File.ReadAllLines(args[0]));
            Console.WriteLine($"2019=>{ret4}");

            var ret2=Deck.GetCardAfterNIteration(BigInteger.Parse("119315717514047"),BigInteger.Parse("101741582076661"), 2020, System.IO.File.ReadAllLines(args[0]));
            Console.WriteLine($"Result part 2 : {ret2}");
            var ret5=Deck.GetLocationAfterNIteration(BigInteger.Parse("119315717514047"),BigInteger.Parse("101741582076661"), ret2, System.IO.File.ReadAllLines(args[0]));
            Console.WriteLine($"Check result part 2 : {ret5}");
        }
    }
}
