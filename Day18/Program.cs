using System;
using System.Linq;
using System.Collections.Generic;
using Utils;

namespace Day18
{
    public class Maze
    {
        public char[][] Data {get;set;}
        public int Width {get; private set;}
        public int Height {get; private set;}
        public Maze(char[][] data)
        {
            Data=data;
            Width=data[0].Length;
            Height=data.Length;
            mazeCells=Each((x,y,c)=>c!='#').Select(i=>(i.x,i.y)).ToArray();
        }

        (int x, int y)[] mazeCells;

        public static char[][] Parse(string[] data)=>data.Select(x=>x.ToCharArray()).ToArray();

        public char this[int x,int y] {
            get {
                return Data[y][x];
            }
            set {
                Data[y][x]=value;
            }
        }

        public IEnumerable<(int x,int y, char c)> Each(Func<int,int,char,bool> predicate)
        {
            for (int i=0;i<Width;i++)
                for (int j=0;j<Width;j++)
                    if (predicate?.Invoke(i,j,this[i,j])??true)
                        yield return (i,j,this[i,j]);
        }

        public string Dump()
        {
            return string.Join("",Data.Select(x=>new string(x)+Environment.NewLine));
        }


        public int dist((int x,int y) a, (int x,int y) b)=>Math.Abs(a.x-b.x)+Math.Abs(a.y-b.y);

        public IEnumerable<(int x, int y)> GetNeighbor((int x,int y) p, char target)
        {
            var (x,y)=p;
            if (this[x-1,y]=='.') yield return (x-1,y);
            else if (char.IsLower(this[x-1,y])) yield return (x-1,y);
            else if (this[x-1,y]==target) yield return (x-1,y);
            if (this[x+1,y]=='.') yield return (x+1,y);
            else if (char.IsLower(this[x+1,y])) yield return (x+1,y);
            else if (this[x+1,y]==target) yield return (x+1,y);
            if (this[x,y-1]=='.') yield return (x,y-1);
            else if (char.IsLower(this[x,y-1])) yield return (x,y-1);
            else if (this[x,y-1]==target) yield return (x,y-1);
            if (this[x,y+1]=='.') yield return (x,y+1);
            else if (char.IsLower(this[x,y+1])) yield return (x,y+1);            
            else if (this[x,y+1]==target) yield return (x,y+1);
        }

        public IEnumerable<(int x, int y)> GetNeighbor((int x,int y) p)
        {
            var (x,y)=p;
            if (this[x-1,y]!='#') yield return (x-1,y);
            if (this[x+1,y]!='#') yield return (x+1,y);
            if (this[x,y-1]!='#') yield return (x,y-1);
            if (this[x,y+1]!='#') yield return (x,y+1);
         }

        public int? CalcDist(char from, char to)
        {
            var start=Each((x,y,c)=>c==from).SingleOrDefault();
            var dest=Each((x,y,c)=>c==to).SingleOrDefault();
            if (start.x==0 || dest.x==0) return null;
            var path=Utils.Dijkstra.FindBestPath<(int x,int y), Utils.Dijkstra.IntCost>(
                mazeCells,
                (start.x,start.y), (dest.x,dest.y),
                a=>GetNeighbor(a,to),
                (a,b)=>1
            );
            var d=path.LastOrDefault();
            if (d.Node.x==0||d.Cost.Value==int.MaxValue)
                return null;
            return d.Cost.Value;
        }

        public int? FindBestPath((int x,int y) start, (int x,int y) dest)
        {
            var path=Utils.Dijkstra.FindBestPath<(int x,int y), Utils.Dijkstra.IntCost>(
                Each((x,y,c)=>c!='#').Select(i=>(i.x,i.y)),
                start, dest,
                a=>GetNeighbor(a,this[dest.x,dest.y]),
                (a,b)=>1
            );

            //Console.WriteLine($"Queue is empty ! {prevs.Count} {dists.Count}");            
            var s=path.LastOrDefault();
            return path.Any()?path.LastOrDefault().Cost.Value:(int?)null;
            //foreach (var a in dists) if (a.Value<1000000) this[a.Key.x,a.Key.y]='+';
            /*while (s!=start) {
                s=prevs[s];
                this[s.x,s.y]='*';
            }*/
            //Console.WriteLine(Dump());
        }

        public int? DistanceToTarget((int x,int y) start, char target)
        {
            //Console.WriteLine($"Search target {target} from {start}");
            var dest=Each((x,y,c)=>c==target).Single();
            var ret=FindBestPath((start.x,start.y),(dest.x, dest.y));
            /*if (ret!=null)
                Console.WriteLine($"Candidate target {target} at {dest} from {start} => dist={ret}");*/
            return ret;
        }

        public (char key, int? dist) ChooseNextKey(IEnumerable<(char key, int? dist)> availableKeys)
        {
            return availableKeys.OrderBy(x=>x.dist).First();
        }

        public class TestItem
        {
            public string KeyPath;
            public char Key;
            public int? Dist;
            public bool Completed;
            public bool Tested;
            public bool AllChildrenTested;
            public bool Initialized;

            public TestItem Parent;

            public List<TestItem> Children;
        }
        public void Dump(TestItem item, int level=0)
        {
            Console.WriteLine($"{new string(' ',level)}{item.Key} {item.Dist} {item.Tested} {item.AllChildrenTested}");
            if (item.Initialized)
                foreach(var i in item.Children)
                    Dump(i,level+1);
        }

        public void Recalc(TestItem item)
        {
            if (item.Initialized && !item.AllChildrenTested) {
                bool flag=true;
                foreach(var i in item.Children)
                {
                    Recalc(i);
                    if (!i.AllChildrenTested) flag=false;
                }
                item.AllChildrenTested=flag;
            }
                    
        }

        public (string,int,int) Solve(TestItem root, int currentMinDist)
        {
            var start=Each((x,y,c)=>c=='@').Single();
            //Console.WriteLine($"starting at {start}");

            int totalDist=0;
            string orderedKeys="";
            TestItem item=root;

            int counter=0;

            List<char> keys="abcdefghijklmnopqrstuvwxyz".ToList();

            while (keys.Count>0) {
                if (!item.Initialized) {
                    item.Children=keys.Select(x=>(x,DistanceToTarget((start.x,start.y),x))).Where(x=>x.Item2!=null).OrderBy(x=>x.Item2.Value).Select(x=>new TestItem { KeyPath=orderedKeys, Key=x.x, Dist=x.Item2, Parent=item }).ToList();
                    item.Initialized=true;
                }
                //Console.WriteLine($"{orderedKeys} availableKeys={new string(item.Children.Where(x=>!x.Tested).Select(x=>x.Key).ToArray())}");
                var next=item.Children.FirstOrDefault(x=>!x.Tested);
                if (next==null) {
                    next=item.Children.FirstOrDefault(x=>!x.AllChildrenTested);
                    if (next==null) {
                        //Console.WriteLine($"{orderedKeys} depleted");
                        item.AllChildrenTested=true;
                        return ("",-1,counter);
                    }        
                }
                item=next;
                item.Tested=true;
                totalDist+=item.Dist.Value;
                if (totalDist>currentMinDist) { item.AllChildrenTested=true; /*Console.WriteLine($"{orderedKeys} skipped => {totalDist}");*/ return ("", -2, counter); }
                //Console.WriteLine($"{orderedKeys} Key {item.Key}, totalDist={totalDist} ({item.Tested})");
                var doorLocation=Each((x,y,c)=>c==char.ToUpper(item.Key)).Single();
                //Console.WriteLine($"Opening door at {doorLocation}");
                this[doorLocation.x,doorLocation.y]='.'; // open door
                var keyLocation=Each((x,y,c)=>c==item.Key).Single();
                this[keyLocation.x,keyLocation.y]='.'; // remove key
                keys.Remove(item.Key); // remove key;
                orderedKeys+=item.Key;
                start=keyLocation;     
                counter++;           
            }
            item.Completed=true;
            item.AllChildrenTested=true;
            Console.WriteLine($"{orderedKeys} completed => {totalDist}");

            return (orderedKeys, totalDist, counter);
        }

    }

    class DistanceMatrix
    {
        public int?[,] Distances{get;}=new int?[53,53];

        public int GetIndex(char c)
        {
            if (c=='@') return 0;
            if (char.IsLower(c)) 
            {
                return 1+Convert.ToInt32(c)-Convert.ToInt32('a');
            } else 
                return 27+Convert.ToInt32(c)-Convert.ToInt32('A');
        }

        public void Dump()
        {
            Console.Write("  ");
            foreach(var i in Program.Keys)
                Console.Write($"{i,4}");
            Console.WriteLine();
            foreach(var j in Program.Keys)
            {
                Console.Write($"{j} ");
                foreach(var i in Program.Keys)
                    Console.Write($"{Distances[GetIndex(i),GetIndex(j)],4}");
                Console.WriteLine();
            }
        }
    }

    class Program
    {
        //public static List<char> Keys="@abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList();
        public static List<char> Keys="@dglsvzrchntaequbpwkjfmyxoiVZCGNELQWBSAHFJIKDMOPRTUXY".ToList();
       
 
        static void Main(string[] args)
        {
            var root=new Maze.TestItem();

            var file=System.IO.File.ReadAllLines(args[0]);
            var initData=Maze.Parse(file);
            var copy=Maze.Parse(file);
            Maze m=new Maze(initData);

            Console.WriteLine(m.Dump());
/*            var doorLocation=m.Each((x,y,c)=>c=='V').Single();
            m[doorLocation.x,doorLocation.y]='.';
            doorLocation=m.Each((x,y,c)=>c=='Z').Single(); // get access to CGNL
            m[doorLocation.x,doorLocation.y]='.';
            doorLocation=m.Each((x,y,c)=>c=='C').Single(); // get access to eS
            m[doorLocation.x,doorLocation.y]='.';
            doorLocation=m.Each((x,y,c)=>c=='G').Single(); // get access to B
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='N').Single(); // get access to 
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='L').Single(); // get access to 
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='E').Single(); // get access to qQ
            m[doorLocation.x,doorLocation.y]='.';             
            doorLocation=m.Each((x,y,c)=>c=='Q').Single(); // get access to ubUB
            m[doorLocation.x,doorLocation.y]='.';               
            doorLocation=m.Each((x,y,c)=>c=='U').Single(); // get access to 
            m[doorLocation.x,doorLocation.y]='.';               
            doorLocation=m.Each((x,y,c)=>c=='B').Single(); // get access to 
            m[doorLocation.x,doorLocation.y]='.';   
            doorLocation=m.Each((x,y,c)=>c=='A').Single(); // get access to pwH
            m[doorLocation.x,doorLocation.y]='.';                           
            doorLocation=m.Each((x,y,c)=>c=='S').Single(); // get access to D
            m[doorLocation.x,doorLocation.y]='.';                           
            doorLocation=m.Each((x,y,c)=>c=='H').Single(); // get access to T
            m[doorLocation.x,doorLocation.y]='.';                           
            doorLocation=m.Each((x,y,c)=>c=='W').Single(); // get access to P
            m[doorLocation.x,doorLocation.y]='.';                           
            doorLocation=m.Each((x,y,c)=>c=='P').Single(); // get access to 
            m[doorLocation.x,doorLocation.y]='.';                           
            doorLocation=m.Each((x,y,c)=>c=='D').Single(); // get access to 
            m[doorLocation.x,doorLocation.y]='.';   
            doorLocation=m.Each((x,y,c)=>c=='T').Single(); // get access to R
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='R').Single(); // get access to kK
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='K').Single(); // get access to jJ
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='J').Single(); // get access to fF
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='F').Single(); // get access to mM
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='M').Single(); // get access to yY
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='Y').Single(); // get access to xX
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='X').Single(); // get access to oO
            m[doorLocation.x,doorLocation.y]='.'; 
            doorLocation=m.Each((x,y,c)=>c=='O').Single(); // get access to iI
            m[doorLocation.x,doorLocation.y]='.'; 
*/
            var distanceMatrix=new DistanceMatrix();
            foreach (var f in Keys)
                foreach (var t in Keys)
                    if (f!=t)
                        distanceMatrix.Distances[distanceMatrix.GetIndex(f),distanceMatrix.GetIndex(t)]=m.CalcDist(f,t);

            distanceMatrix.Dump();

/*
            string prevPath="";
            string minPath="";
            int min=int.MaxValue;
            do {
                for(int i=0;i<m.Data.Length;i++) Array.Copy(copy[i],m.Data[i],copy[i].Length);
                var (path,len,c)=m.Solve(root, min);
                if (len>=0) {
                    if (len<min)
                    {
                        min=len;
                        minPath=path;
                    } else {
                        if (path==prevPath) break;
                        prevPath=path;
                    }
                }
                m.Recalc(root);
            } while(!root.AllChildrenTested);
            //m.Dump(root);
            */
        }
    }
}
