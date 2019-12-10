using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day10
{
    public class AsteroidField
    {
        public AsteroidField(string[] data)
        {
            this.data=new Cell[data[0].Length,data.Length];
            foreach(var (line,y) in data.Zip(Enumerable.Range(0,data.Length)))
                foreach(var (cell,x) in line.Zip(Enumerable.Range(0,line.Length)))
                    this.data[x,y]=new Cell { X=x, Y=y, HasAsteroid=cell=='#' };
        }

        Cell[,] data;

        public Cell GetCell(Cell relative, int x, int y)
        {
            int X=(relative?.X??0)+x;
            int Y=(relative?.Y??0)+y;
            if (X<0 || Y<0 || X>=data.GetLength(0) || Y>=data.GetLength(1))
                return new Cell { X=X, Y=Y, IsInfiniteVoid=true };
            return data[X,Y];
        }

        public class Cell
        {
            public bool HasAsteroid {get;set;}
            public int X {get;set;}
            public int Y {get;set;}

            public bool IsBlocked {get;set;}
            public bool IsVisited {get;set;}
            public bool IsDestroyed {get;set;}
            public bool IsCandidate {get;set;}
            public bool IsInfiniteVoid {get;set;}

        }

        public (Cell,int) GetMaxVisibilityCell()
        {
            Cell maxCell=null;
            int max=0;
            foreach(var a in data) 
            {
                if (a.HasAsteroid) {
                    var c=CountUnblockedAsteroid(a);
                    if (c>max) {
                        max=c;
                        maxCell=a;
                    }
                }
            }
            return (maxCell,max);
        }

        public int CountUnblockedAsteroid(Cell from)=>GetUnblockedAsteroid(from).Count();

        public bool TraceGetUnblockedAsteroid {get;set;}=false;
        public bool TraceVaporize {get;set;}=false;

        public IEnumerable<Cell> GetUnblockedAsteroid(Cell from)
        {
            foreach(var a in data) { a.IsBlocked=false; a.IsVisited=false; }

            for(int i=1;i<Math.Max(data.GetLength(0),data.GetLength(1));i++) 
            {
                if (TraceGetUnblockedAsteroid) Log?.Invoke($"Radius = {i}");
                foreach(var (x,y) in EnumSquare(i)) 
                {
                    var cell=GetCell(from,x, y);
                    if (cell.HasAsteroid) {
                        var p=pgcd(x,y);
                        var ax=x/p;
                        var ay=y/p;
                        cell=GetCell(cell,ax,ay);
                        while (!cell.IsInfiniteVoid) {
                            cell.IsBlocked=true;
                            cell=GetCell(cell,ax,ay);
                        }
                    }
                    cell.IsVisited=true;
                }
                if (TraceGetUnblockedAsteroid) Dump(from);
            }

            return EnumArray(data).Where(a=>a!=from && a.HasAsteroid && !a.IsBlocked).OrderBy(a=>(Math.PI+Math.Atan2(a.Y-from.Y,a.X-from.X)+3*Math.PI/2)%(2*Math.PI));

        }

        public IEnumerable<Cell> VaporizeOneRound(Cell from)
        {
            foreach (var a in data) a.IsVisited=false;
            //Dump(from,c=>c.IsVisited?'*':' ');
            var visibles=GetUnblockedAsteroid(from).ToArray();
            foreach (var asteroid in visibles) { asteroid.IsCandidate=true; }
            //Dump(from, true);
            foreach(var asteroid in visibles)
            {
                asteroid.HasAsteroid=false;
                asteroid.IsDestroyed=true;
                asteroid.IsCandidate=false;
                if (TraceVaporize) {
                    Log?.Invoke($"Vaporized {asteroid.X},{asteroid.Y} : ");
                    //Dump(from, c=>c.IsDestroyed?'+':'.');
                    //Dump(from, true);
                }
                yield return asteroid;
            }
            Dump(from, true);
        }

        public IEnumerable<Cell> Vaporize(Cell from)
        {
            int roundCount=0;
            int oneRoundCount;
            do {
                roundCount++;
                oneRoundCount=0;
                if (TraceVaporize) Log?.Invoke($"Round {roundCount}");
                foreach(var asteroid in VaporizeOneRound(from)) {
                    oneRoundCount++;
                    yield return asteroid;
                }
            } while (oneRoundCount!=0);

        }

        static IEnumerable<Cell> EnumArray(Cell[,] data) {
             foreach(var a in data) yield return a;
        }

        public Action<string> Log {get;set;}

        public static int pgcd(int a, int b)
        {
            int x = Math.Abs(a);
            int y = Math.Abs(b);
            int m = Math.Max(x,y);
            for (int i = m; i >= 1; i--)
            {
                if (x % i == 0 && y % i == 0)
                {
                    return i;
                }
            }
            return 1;
        }

        public void Dump(Cell from, bool simple=false)
        {
            if (simple)
                Dump(from, d=> {
                    if (d.IsCandidate)    
                        return '+';
                    else if (d.IsDestroyed)    
                        return '*';
                    else if (d.HasAsteroid)    
                        return '#';
                    else
                        return '.';
                });
            else
                Dump(from, d => {
                    if (d.IsVisited) {
                        if (d.HasAsteroid && d.IsBlocked)
                            return 'X';
                        else if (d.HasAsteroid && !d.IsBlocked)
                            return '+';
                        else if (!d.HasAsteroid && !d.IsBlocked)
                            return '.';
                        else if (!d.HasAsteroid && d.IsBlocked)
                            return ':';
                    } else {
                        if (d.HasAsteroid && d.IsBlocked)
                            return 'x';
                        else if (d.HasAsteroid && !d.IsBlocked)
                            return '#';
                        else if (!d.HasAsteroid && !d.IsBlocked)
                            return ' ';
                        else if (!d.HasAsteroid && d.IsBlocked)
                            return '_';
                    }
                    return ' ';
                });
        }

        public void Dump(Cell from, Func<Cell,char> display)
        {
            var s=new StringBuilder();
            s.Append('\n');
            for(int y=0;y<data.GetLength(1);y++)
            {
                for(int x=0;x<data.GetLength(0);x++)
                {
                    var d=data[x,y];                    
                    if (d==from) s.Append('O');
                    else s.Append(display(d));
                }
                s.Append('\n');
            }
            Log?.Invoke(s.ToString());
        }

        private IEnumerable<(int x,int y)> EnumSquare(int radius)
        {
            for (int x=-radius;x<radius;x++) yield return (x,radius);
            for (int y=radius;y>-radius;y--) yield return (radius,y);
            for (int x=radius;x>-radius;x--) yield return (x,-radius);
            for (int y=-radius;y<radius;y++) yield return (-radius,y);
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            var map=@"#.#....#.#......#.....#......####.
#....#....##...#..#..##....#.##..#
#.#..#....#..#....##...###......##
...........##..##..##.####.#......
...##..##....##.#.....#.##....#..#
..##.....#..#.......#.#.........##
...###..##.###.#..................
.##...###.#.#.......#.#...##..#.#.
...#...##....#....##.#.....#...#.#
..##........#.#...#..#...##...##..
..#.##.......#..#......#.....##..#
....###..#..#...###...#.###...#.##
..#........#....#.....##.....#.#.#
...#....#.....#..#...###........#.
.##...#........#.#...#...##.......
.#....#.#.#.#.....#...........#...
.......###.##...#..#.#....#..##..#
#..#..###.#.......##....##.#..#...
..##...#.#.#........##..#..#.#..#.
.#.##..#.......#.#.#.........##.##
...#.#.....#.#....###.#.........#.
.#..#.##...#......#......#..##....
.##....#.#......##...#....#.##..#.
#..#..#..#...........#......##...#
#....##...#......#.###.#..#.#...#.
#......#.#.#.#....###..##.##...##.
......#.......#.#.#.#...#...##....
....##..#.....#.......#....#...#..
.#........#....#...#.#..#....#....
.#.##.##..##.#.#####..........##..
..####...##.#.....##.............#
....##......#.#..#....###....##...
......#..#.#####.#................
.#....#.#..#.###....##.......##.#.";

            var field=new AsteroidField(map.Split(new char[] {'\n','\r'},100,StringSplitOptions.RemoveEmptyEntries));
            var (cell,c) = field.GetMaxVisibilityCell();

            Console.WriteLine($"{cell.X} {cell.Y} => {c}");
            //field.Log=Console.WriteLine;
            //field.TraceVaporize=true;
            int i=0;
            foreach(var asteroid in field.Vaporize(cell))
            {
                i++;
                if (i==200) {
                    Console.WriteLine($"{i} Vaporized {asteroid.X},{asteroid.Y} => {asteroid.X*100+asteroid.Y}");                
                }
            }

        }
    }
}
