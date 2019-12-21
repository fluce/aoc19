using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class Helper
    {
        public static IEnumerable<(int Idx, T Item)> Index<T>(this IEnumerable<T> e)
        {
            int i=0;
            foreach(var a in e) yield return (i++,a);
        }

        public static readonly (int X, int Y)[] AdjacentPoints = { (0,-1),(-1,0),(0,1),(1,0) };

        public static IEnumerable<(int X,int Y)> Adjacents(this (int X,int Y) p) {
            return AdjacentPoints.Select(q=>(q.X+p.X,q.Y+p.Y));
        }

    }
}
