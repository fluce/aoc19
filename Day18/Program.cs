using System;
using System.Linq;
using System.Collections.Generic;
using Utils;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Day18
{

    public class Item
    {
        public char? Key {get;set;}
        public char? Door {get;set;}
        public bool Start {get;set;}
    }

    public class Node
    {
        public char Key {get;set;}
        public int Distance {get;set;}
        public string Doors {get;set;}
   
        public override string ToString() => $"{Key} d={Distance} doors={Doors}";
    }
    public class Maze
    {
        Table<Item> Data;
        Dictionary<char,(int x,int y)> Keys=new Dictionary<char, (int x, int y)>();

        Dictionary<char, List<Node>> Graph=new Dictionary<char, List<Node>>();

        public Maze(string[] input)
        {
            Data=Table<Item>.Parse(input, (p,c)=>
                c switch {
                    '#'=>null,
                    '.'=>new Item(),
                    '@'=>new Item() { Start=true },
                    _ => new Item() { Key=char.IsLower(c)?c:(char?)null, Door=char.IsUpper(c)?c:(char?)null }
                }
            );
            Keys=Data.Each((x,y,i)=>i?.Key.HasValue??false).ToDictionary(x=>x.Item.Key.Value, x=>(x.X,x.Y));
            int n=0;
            foreach(var a in Data.Each((x,y,i)=>i?.Start??false)) Keys[Convert.ToChar(48+(n++))]=(a.X,a.Y);

            foreach(var k in Keys) {
                Graph[k.Key]=BuildGraph(k.Value).ToList();
                Console.WriteLine($"Graph '{k.Key}'");
                foreach(var a in Graph[k.Key].OrderBy(x=>x.Key))
                    Console.WriteLine($"  {a}");
            }
        }

        readonly (int dx,int dy)[] directions = new (int dx,int dy)[] { (0,1),(1,0),(0,-1),(-1,0) };

        public IEnumerable<Node> BuildGraph((int x,int y) key)
        {
            var visited=new HashSet<(int x,int y)>();
            var distance=new Dictionary<(int x,int y),int>();
            var doors=new Dictionary<(int x,int y),string>();
   
            visited.Add(key);
            distance[key]=0;
            doors[key]="";
            var queue=new Queue<(int x,int y)>();
            queue.Enqueue(key);
            while (queue.TryDequeue(out var u))
            {
                foreach(var d in directions)
                {
                    var v=(x:u.x+d.dx, y:u.y+d.dy);
                    var item=Data[v];
                    
                    if (item==null)
                        continue;
                    
                    if (visited.Contains(v))
                        continue;

                    visited.Add(v);

                    distance[v]=distance[u]+1;
                    doors[v]=doors[u]+item.Door??""; // update door list on path
   
                    if (item.Key!=null) 
                    {
                        yield return new Node {
                            Key=item.Key.Value,
                            Distance=distance[v],
                            Doors=doors[v],
                        };
                    }
                    queue.Enqueue(v);
                }
            }
        }

        public int CalcShortestPathLength()
        {
            var allStarts=Keys.Where(x=>char.IsDigit(x.Key)).Select(x=>x.Key).ToList();
            // calculate sum of each path, given all keys from other zone have been discovered
            return allStarts.Sum(
                        x => CalcShortestPathLength(
                            x, 
                            string.Join("",new HashSet<char>(
                                                allStarts.Except(new[]{x})
                                                         .SelectMany(y=>Graph[y].Select(z=>z.Key))
                                            ))
                        )
                    );
        }

        Dictionary<string,int> cache=new Dictionary<string, int>();

        public int CalcShortestPathLength(char from, string visited, int depth=0)
        {
            //Check if already calculated
            string cache_key=$"{from}/{string.Join("",visited.OrderBy(c=>c))}";
            if (cache.TryGetValue(cache_key,out var ret)) return ret;

            // calc nodes that are reachable from there
            var reachable=new List<Node>();
            foreach(var node in Graph[from]) {
                // already visited
                if (visited.Contains(node.Key)) continue; 
                // some doors on the way are not opened
                if (node.Doors.Select(x=>char.ToLower(x)).Intersect(visited).Count()!=node.Doors.Count()) continue;
                reachable.Add(node);
            }
            int min=int.MaxValue;

            if (reachable.Count()==0) {
                min=0;
            } else {
                foreach(var r in reachable)
                {
                    //Console.WriteLine($"B{new string(' ',depth)} Shortest from {from} visited={visited} => {r.Key}-{visited+r.Key} => {r.Distance}");
                    var d=CalcShortestPathLength(r.Key,visited+r.Key,depth+1);
                    var dist=r.Distance+d ;
                    if (dist<min) {
                        //Console.WriteLine($"E{new string(' ',depth)} Shortest from {from} visited={visited} => {r.Key}-{visited+r.Key} => {r.Distance}+{d} = {dist} {(dist<min?"MIN":"")}");
                        min=dist;
                    }
                }
            }
            cache[cache_key]=min;
            return min;
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            var file=System.IO.File.ReadAllLines(args[0]);
            Maze m=new Maze(file);
            var part1=m.CalcShortestPathLength();            
            Console.WriteLine($"Shortest path={part1}");
            file[39]=file[39].Remove(39,3).Insert(39,"@#@");
            file[40]=file[40].Remove(39,3).Insert(39,"###");
            file[41]=file[41].Remove(39,3).Insert(39,"@#@");
            m=new Maze(file);
            var part2=m.CalcShortestPathLength();
            Console.WriteLine($"Shortest path part 1={part1}");
            Console.WriteLine($"Shortest path part 2={part2}");
}
    }
}
