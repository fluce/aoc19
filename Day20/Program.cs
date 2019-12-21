using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Day20
{
    public class Item
    {
        public char Value {get;set;}

        public string Portal {get;set;}
        public bool GoingDeeper {get;set;}

        public override string ToString()
        {
            //if (Portal!=null) return Portal;
            if (Portal!=null) return GoingDeeper?"v":"^";
            return Value.ToString();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Table<Item> table=Table<Item>.Parse(System.IO.File.ReadAllLines(args[0]),(p,c)=>new Item { Value=c });

            foreach (var item in table.Each((p,i)=>char.IsLetter(i.Value)))
            {
                var (idx,p,ai)=item.Position.Adjacents().Index().Select(x=>(x.Idx,x.Item,table[x.Item])).Where(x=>x.Item3?.Value=='.').SingleOrDefault();
                if (ai!=null) 
                { 
                    Console.WriteLine($"item {item.Position}=>{item.Item}");
                    Console.WriteLine($"ai   {p}=>{ai}");
                    var p2=(2*item.Position.X-p.X,2*item.Position.Y-p.Y);
                    Console.WriteLine($"p2   {2}=>{table[p2].Value}");
                    if (idx<2)
                        ai.Portal=item.Item.Value.ToString()+table[p2].Value;
                    else
                        ai.Portal=table[p2].Value+item.Item.Value.ToString();
                    ai.GoingDeeper=(p.X>5 && p.X<table.Width-5 && p.Y>5 && p.Y<table.Height-5);

                }
            }
            var portals=table.Each((p,i)=>i.Portal!=null).GroupBy(x=>x.Item.Portal).ToDictionary(x=>x.Key,x=>x.ToList());
            var portals2=new Dictionary<(int X,int Y),(int X,int Y)>();
            
            foreach (var i in portals) {
                Console.WriteLine($"{i.Key} {string.Join(" ",i.Value.Select(x=>x.Position.ToString()))}");
                if (i.Value.Count==2) {
                    portals2[i.Value[0].Position]=i.Value[1].Position;
                    portals2[i.Value[1].Position]=i.Value[0].Position;
                }
            }
            Console.WriteLine("Portals2");
            foreach (var i in portals2) Console.WriteLine($"{i.Key}=>{i.Value}");

            foreach (var it in portals.SelectMany(x=>x.Value)) {
                //Console.WriteLine($"Portal {it.Item.Portal} {it.Position}: ");
                var res=Dijkstra.Fill<(int X,int Y),Dijkstra.IntCost>(table.Each((p,i)=>i.Value=='.').Select(x=>x.Position),it.Position,p=>p.Adjacents().Where(x=>table[x].Value=='.'),(x,y)=>1);
                foreach(var i in res)
                {
                    //table[i.Node].Value='*';
                    if (table[i.Node].Portal!=null && i.Cost.Value!=0)
                        Console.WriteLine($" {table[i.Node].Portal} {i.Node} <=> {it.Item.Portal} {it.Position} - {i.Cost.Value}");
                }
            }

            IEnumerable<(int X,int Y)> GetPortal((int X,int Y) pos) {
                if (portals2.TryGetValue(pos, out var ret)) yield return ret;
            }

            Console.WriteLine("Result :");
            var result=Dijkstra.FindBestPath<(int X,int Y),Dijkstra.IntCost>(
                        table.Each((p,i)=>i.Value=='.').Select(x=>x.Position),
                        portals["AA"].First().Position,portals["ZZ"].First().Position,
                        p=>p.Adjacents().Where(x=>table[x].Value=='.').Concat(GetPortal(p)),
                        (x,y)=>1);
           /* foreach(var i in result)
            {
                //table[i.Node].Value='*';
                Console.WriteLine($" {i.Node}  {i.Cost.Value}");
            }*/
            Console.WriteLine(table.Dump());
            Console.WriteLine($"Part 1 result : {result.Last().Cost.Value}");

            var result2=Dijkstra.FindBestPath<((int X,int Y) Position,int Level),Dijkstra.IntCost>(
                        Enumerable.Range(0,30).SelectMany(level=>table.Each((p,i)=>i.Value=='.').Select(x=>(x.Position,level))),
                        (portals["AA"].First().Position,0),(portals["ZZ"].First().Position,0),
                        p=>p.Position.Adjacents()
                                .Where(x=>table[x].Value=='.')
                                .Select(x=>(x,p.Level))
                                .Concat(
                                    GetPortal(p.Position)
                                        .Where(x=>(table[p.Position].Portal=="AA" || table[p.Position].Portal=="ZZ")?p.Level==0:(p.Level!=0||table[p.Position].GoingDeeper))
                                        .Select(x=>(x, table[p.Position].GoingDeeper?p.Level+1:p.Level-1 )))
                                /*.Select( x=> { 
                                    var d=table[p.Position]; 
                                    if(d.Portal!=null && x.Item2!=p.Level) 
                                      Console.WriteLine($"{p.Position} {p.Level} => {d.Portal} {d.GoingDeeper} {x.Item1} {x.Item2}");  
                                    return x;})*/,
                        (x,y)=>1);
            foreach(var i in result2)
            {
                //table[i.Node].Value='*';
                //Console.WriteLine($" {i.Node.Level} {i.Node.Position} {table[i.Node.Position].Portal??""} {i.Cost.Value}");
            }
            //Console.WriteLine(table.Dump());
            Console.WriteLine($"Part 2 result : {result2.Last().Cost.Value}");

        }
    }
}
