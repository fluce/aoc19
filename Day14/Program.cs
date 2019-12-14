using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Day14
{
    public class Reaction
    {
        public (int q, string elm) Output {get;set;}
        public (int q, string elm)[] Inputs {get;set;}

        private static Regex regex=new Regex(@"((?<q>\d+) (?<e>[A-Z]+)[, ]*)+ => (?<qo>\d+) (?<eo>[A-Z]+)");
        public static List<Reaction> Parse(string description)
        {
            var matches=regex.Matches(description);
            return matches.Select(x=>new Reaction() {
                Inputs=x.Groups["q"].Captures.Zip(x.Groups["e"].Captures).Select(y=>(int.Parse(y.First.Value),y.Second.Value)).ToArray(),
                Output=x.Groups["qo"].Captures.Zip(x.Groups["eo"].Captures).Select(y=>(int.Parse(y.First.Value),y.Second.Value)).Single()
            } ).ToList() ;
        }

        public override string ToString()
        {
            return string.Join(", ",Inputs.Select(x=>$"{x.q} {x.elm}"))+" => "+$"{Output.q} {Output.elm}";
        }
    }

    public class NanoFactory
    {
        public List<Reaction> Reactions {get;set;}
        public Action<string> Log {get;set;}
        public Action<string> Log2 {get;set;}

        public NanoFactory(string description)
        {
            Reactions=Reaction.Parse(description);
        }

        public Reaction WhatsNeededStep(string elm)
        {
            if (elm=="ORE") return null;
            var reaction=Reactions.Single(x=>x.Output.elm==elm);
            return reaction;
        }

        public Dictionary<string, long> Supply=new Dictionary<string, long>();
        public void AddToSupply(long q, string elm)
        {
            long q2=0;
            Supply.TryGetValue(elm,out q2);
            Supply[elm]=q+q2;
        }
        public void DumpSupply()
        {
            Log?.Invoke(string.Join(" - ",Supply.Select(x=>$"{x.Value} {x.Key}")));
        }

        public void WhatsNeeded(long qty, string target)
        {
            if (qty<=0) return;
            var en=WhatsNeededStep(target);        
    
            if (en!=null) {                
                Log?.Invoke($"Need {qty} {target} will use {en?.ToString()}");
                long factor=(en.Output.q>=qty)?1:(long)Math.Ceiling(1.0*qty/en.Output.q);
                Log?.Invoke($"Factor is {factor}");
                AddToSupply(factor*en.Output.q, en.Output.elm);
                foreach(var r in en.Inputs) 
                    AddToSupply(-factor*r.q, r.elm);
                DumpSupply();
                foreach(var r in en.Inputs) {
                    WhatsNeeded(-Supply[r.elm], r.elm);
                } 
            }
        }

        public long HowMuchOreNeededForFuel(long qty)
        {
            Supply.Clear();
            Log2?.Invoke($"HowMuchOreNeededForFuel {qty}");
            WhatsNeeded(qty,"FUEL");
            if (Supply.TryGetValue("ORE",out var res)) return -res;
            return 0;
        }

        public long HowMuchFuelForOre(long ore, long tryBottom, long tryTop, int maxStep=100)
        {
            long _try=(tryBottom+tryTop)/2;
            if (tryBottom==_try || maxStep==0)
                return _try;
            long result=HowMuchOreNeededForFuel(_try);
            Log2?.Invoke($"{tryBottom} - {tryTop} => {_try} : {result} ({result*100.0/ore}%)");

            if (result>ore) 
                return HowMuchFuelForOre(ore,tryBottom,_try, maxStep-1);
            if (result<ore) 
                return HowMuchFuelForOre(ore,_try,tryTop, maxStep-1);
            else return _try;
        }
    }

    class Program
    {        
        static void Main(string[] args)
        {
            var factory=new NanoFactory(@"1 ZQVND => 2 MBZM
2 KZCVX, 1 SZBQ => 7 HQFB
1 PFSQF => 9 RSVN
2 PJXQB => 4 FSNZ
20 JVDKQ, 2 LSQFK, 8 SDNCK, 1 MQJNV, 13 LBTV, 3 KPBRX => 5 QBPC
131 ORE => 8 WDQSL
19 BRGJH, 2 KNVN, 3 CRKW => 9 MQJNV
16 DNPM, 1 VTVBF, 11 JSGM => 1 BWVJ
3 KNVN, 1 JQRML => 7 HGQJ
1 MRQJ, 2 HQFB, 1 MQJNV => 5 VQLP
1 PLGH => 5 DMGF
12 DMGF, 3 DNPM, 1 CRKW => 1 CLML
1 JSGM, 1 RSVN => 5 TMNKH
1 RFJLG, 3 CFWC => 2 ZJMC
1 BRGJH => 5 KPBRX
1 SZBQ, 17 GBVJF => 4 ZHGL
2 PLGH => 5 CFWC
4 FCBZS, 2 XQWHB => 8 JSGM
2 PFSQF => 2 KNVN
12 CRKW, 9 GBVJF => 1 KRCB
1 ZHGL => 8 PJMFP
198 ORE => 2 XQWHB
2 BWVJ, 7 CFWC, 17 DPMWN => 3 KZCVX
4 WXBF => 6 JVDKQ
2 SWMTK, 1 JQRML => 7 QXGZ
1 JSGM, 1 LFSFJ => 4 LSQFK
73 KNVN, 65 VQLP, 12 QBPC, 4 XGTL, 10 SWMTK, 51 ZJMC, 4 JMCPR, 1 VNHT => 1 FUEL
1 BWVJ, 7 MBZM => 5 JXZT
10 CFWC => 2 DPMWN
13 LQDLN => 3 LBTV
1 PFZW, 3 LQDLN => 5 PJXQB
2 RSVN, 2 PFSQF => 5 CRKW
1 HGQJ, 3 SMNGJ, 36 JXZT, 10 FHKG, 3 KPBRX, 2 CLML => 3 JMCPR
126 ORE => 4 FCBZS
1 DNPM, 13 MBZM => 5 PLGH
2 XQWHB, 10 FCBZS => 9 LFSFJ
1 DPMWN => 9 PFZW
1 ZJMC, 3 TMNKH => 2 SWMTK
7 TZCK, 1 XQWHB => 5 ZQVND
4 CFWC, 1 ZLWN, 5 RSVN => 2 WXBF
1 BRGJH, 2 CLML => 6 LQDLN
26 BWVJ => 2 GBVJF
16 PJXQB, 20 SDNCK, 3 HQFB, 7 QXGZ, 2 KNVN, 9 KZCVX => 8 XGTL
8 PJMFP, 3 BRGJH, 19 MRQJ => 5 SMNGJ
7 DNPM => 2 SZBQ
2 JQRML, 14 SDNCK => 8 FHKG
1 FSNZ, 6 RFJLG, 2 CRKW => 8 SDNCK
2 CLML, 4 SWMTK, 16 KNVN => 4 JQRML
8 TZCK, 18 WDQSL => 2 PFSQF
1 LSQFK => 8 VTVBF
18 BRGJH, 8 ZHGL, 2 KRCB => 7 VNHT
3 TZCK => 4 DNPM
14 PFZW, 1 PFSQF => 7 BRGJH
21 PLGH, 6 VTVBF, 2 RSVN => 1 ZLWN
149 ORE => 2 TZCK
3 JSGM => 1 RFJLG
4 PFSQF, 4 DMGF => 3 MRQJ");
            //factory.Log=Console.WriteLine;

            var c=factory.HowMuchOreNeededForFuel(1);
            Console.WriteLine($"Ore needed {c}");

            var res=factory.HowMuchFuelForOre(1000000000000,0,1000000000000);
            Console.WriteLine($"Result = {res}");
        }
    }
}
