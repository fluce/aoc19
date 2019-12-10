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

        public int CountUnblockedAsteroid(Cell from)
        {
            foreach(var a in data) { a.IsBlocked=false; a.IsVisited=false; }

            for(int i=1;i<Math.Max(data.GetLength(0),data.GetLength(1));i++) 
            {
                Log?.Invoke($"Radius = {i}");
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
                Dump(from);
            }

            int count=0;
            foreach(var a in data) if (a!=from && a.HasAsteroid && !a.IsBlocked) count++;
            return count;
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

        public void Dump(Cell from)
        {
            var s=new StringBuilder();
            s.Append('\n');
            for(int y=0;y<data.GetLength(1);y++)
            {
                for(int x=0;x<data.GetLength(0);x++)
                {
                    var d=data[x,y];
                    if (d==from) s.Append('O');
                    else if (d.IsVisited) {
                        if (d.HasAsteroid && d.IsBlocked)
                            s.Append('X');
                        else if (d.HasAsteroid && !d.IsBlocked)
                            s.Append('+');
                        else if (!d.HasAsteroid && !d.IsBlocked)
                            s.Append('.');
                        else if (!d.HasAsteroid && d.IsBlocked)
                            s.Append(':');
                    } else {
                        if (d.HasAsteroid && d.IsBlocked)
                            s.Append('x');
                        else if (d.HasAsteroid && !d.IsBlocked)
                            s.Append('#');
                        else if (!d.HasAsteroid && !d.IsBlocked)
                            s.Append(' ');
                        else if (!d.HasAsteroid && d.IsBlocked)
                            s.Append('_');
                    }
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

            System.Diagnostics.Debugger.Launch();
            var field=new AsteroidField(map.Split(new char[] {'\n','\r'},100,StringSplitOptions.RemoveEmptyEntries));
            var (cell,c) = field.GetMaxVisibilityCell();

            Console.WriteLine($"{cell.X} {cell.Y} => {c}");
        }
    }
}
