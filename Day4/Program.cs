using System;
using System.Linq;
using System.Collections.Generic;

namespace Day4
{
    public class Day4Impl
    {
        public Day4Impl(int min, int max)
        {
            Min=min;
            Max=max;
        }

        public bool Check(int si)
        {
            if (si<Min) return false;
            if (si>Max) return false;
            var s=si.ToString();
            bool adj=false;
            bool dec=false;
            for(int i=0;i<5;i++) {
                if (s[i]==s[i+1]) adj=true;
                if (s[i+1]<s[i]) dec=true;
            }
            return adj && !dec;
        }

        public bool Check2(int si)
        {
            var s="A"+si.ToString()+"A";
            bool adj=false;
            for(int i=1;i<6;i++) {
                if (s[i]!=s[i-1] && s[i]==s[i+1] && s[i+1]!=s[i+2]) adj=true;
            }
            return adj;
        }

        public void Execute()
        {
            var list1=Enumerable.Range(Min,Max-Min+1).Where(Check).ToList();
            foreach (var a in list1.Where(Check2)) Console.WriteLine(a);
            Result1=list1.Count();
            Result2=list1.Count(Check2);
        }

        public int Result1 {get;private set;}
        public int Result2 {get;private set;}
        public int Min {get;private set;}
        public int Max {get;private set;}
    }

    class Program
    {
        static void Main(string[] args)
        {
            var impl=new Day4Impl(367479,893698);
            impl.Execute();
            Console.WriteLine($"Count1={impl.Result1}");
            Console.WriteLine($"Count2={impl.Result2}");
        }
    }
}
