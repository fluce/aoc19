using System;
using System.Linq;
using System.Collections.Generic;
using Utils;

namespace Day18
{

    public class Wall:Item
    {

    }

    public class Item
    {
        public char? Key {get;set;}
        public char? Door {get;set;}
    }

    public class Node
    {
        public char Key {get;set;}
        public int Distance {get;set;}
        public List<char> Doors {get;set;}
        public List<char> Ways {get;set;}

        public override string ToString() => $"{Key} d={Distance} doors={new string(Doors.ToArray())} ways={new string(Ways.ToArray())}";
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
                    _ => new Item() { Key=char.IsLower(c)?c:(char?)null, Door=char.IsUpper(c)?c:(char?)null }
                }
            );
            Keys=Data.Each((x,y,i)=>i?.Key.HasValue??false).ToDictionary(x=>x.Item.Key.Value, x=>(x.X,x.Y));

            foreach(var k in Keys.Keys)
                Graph[k]=ListNeighbors(k).ToList();

            foreach(var k in Keys.Keys) {
                Console.WriteLine($"Graph '{k}'");
                foreach(var a in Graph[k])
                    Console.WriteLine($"  {a}");
            }
        }

        readonly (int dx,int dy)[] directions = new (int dx,int dy)[] { (0,-1),(1,0),(0,1),(-1,0) };

        public IEnumerable<Node> ListNeighbors(char n)
        {
            var visited=new HashSet<(int x,int y)>();
            var distance=new Dictionary<(int x,int y),int>();
            var doors=new Dictionary<(int x,int y),List<char>>();
            var ways=new Dictionary<(int x,int y),List<char>>();

            var k=Keys[n]; // key position
            visited.Add(k);
            distance[k]=0;
            doors[k]=new List<char>();
            ways[k]=new List<char>();
            var stack=new Stack<(int x,int y)>();
            stack.Push(k);
            while (stack.Count>0)
            {
                var u=stack.Pop();
                foreach(var d in directions)
                {
                    var v=(x:u.x+d.dx, y:u.y+d.dy);
                    var item=Data[v];
                    
                    if (item==null)
                        continue;
                    
                    if (visited.Contains(v))
                        continue;

                    distance[v]=distance[u]+1;
                    doors[v]=doors[u].ToList();
                    ways[v]=ways[u].ToList();

                    visited.Add(v);

                    if (item.Door!=null) doors[v].Add(item.Door.Value);

                    if (item.Key!=null) 
                    {
                        yield return new Node {
                            Key=item.Key.Value,
                            Distance=distance[v],
                            Doors=doors[v],
                            Ways=ways[v].ToList()
                        };
                        ways[v].Add(item.Key.Value);
                    }
                    stack.Push(v);
                }
            }
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            var file=System.IO.File.ReadAllLines(args[0]);
            Maze m=new Maze(file);
        }
    }
}
