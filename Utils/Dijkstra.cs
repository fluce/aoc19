using System;
using System.Collections.Generic;
using System.Linq;
using FibonacciHeap;

namespace Utils
{

    public static class Dijkstra
    {
        struct Holder<T>
        {
            public T Value {get;set;}
            public bool IsEmpty {get;set;}

            public static readonly Holder<T> Empty = new Holder<T> { IsEmpty=true };  

            public static Holder<T> From(T value) => new Holder<T> { Value=value };
        }

        public interface ICost<C>: IComparable<C> where C:struct
        {
            C Add(C a);
            C Zero {get;}
            C MaxValue {get;}
        }

        public struct IntCost : ICost<IntCost>
        {
            public IntCost(int value) { Value=value; }

            public int Value { get;set;}

            public IntCost Zero => 0;

            public IntCost MaxValue => Int32.MaxValue;

            public IntCost Add(IntCost a)
            {
                if (Value == Int32.MaxValue || a.Value == Int32.MaxValue)
                    return Int32.MaxValue;
                return Value+a.Value;                
            }

            public int CompareTo(IntCost other)
            {
                return Value.CompareTo(other.Value);
            }

            public static implicit operator IntCost(int a) => new IntCost(a);
        }

        public static IEnumerable<(T Node,C Cost)> Fill<T,C>(
                IEnumerable<T> set, 
                T start,  
                Func<T,IEnumerable<T>> GetNeighbors, 
                Func<T,T,C> GetCost
            ) 
            where T:IEquatable<T> where C:struct, ICost<C>
        {
            var costs = new Dictionary<T, C>();
            var prevs = new Dictionary<T, Holder<T>>();
            InnerFill(set, start, GetNeighbors, GetCost, costs, prevs);
            
            return prevs.Where(x=>!x.Value.IsEmpty).SelectMany(x=>new T[]{x.Key, x.Value.Value }).Distinct().Select(x=>(x,costs[x]));
        }


        public static IEnumerable<(T Node,C Cost)> FindBestPath<T,C>(
                IEnumerable<T> set, 
                T start, T dest, 
                Func<T,IEnumerable<T>> GetNeighbors, 
                Func<T,T,C> GetCost
            ) 
            where T:IEquatable<T> where C:struct, ICost<C>
        {
            var costs = new Dictionary<T, C>();
            var prevs = new Dictionary<T, Holder<T>>();
            InnerFill(set, start, GetNeighbors, GetCost, costs, prevs);
            var path = new LinkedList<(T, C)>();
            var a = Holder<T>.From(dest);
            while (!a.IsEmpty)
            {
                path.AddFirst((a.Value, costs[a.Value]));
                a = prevs[a.Value];
            }
            return path;

        }

        private static void InnerFill<T, C>(IEnumerable<T> set, T start, Func<T, IEnumerable<T>> GetNeighbors, Func<T, T, C> GetCost, Dictionary<T, C> costs, Dictionary<T, Holder<T>> prevs)
            where T : IEquatable<T>
            where C : struct, ICost<C>
        {
            var queue = new FibonacciHeap<T, C>(default(C).Zero);
            var nodes = new Dictionary<T, FibonacciHeapNode<T, C>>();

            costs[start] = default(C).Zero;
            foreach (var item in set)
            {
                if (!start.Equals(item)) costs[item] = default(C).MaxValue;
                prevs[item] = Holder<T>.Empty;
                nodes[item] = new FibonacciHeapNode<T, C>(item, costs[item]);
                queue.Insert(nodes[item]);
            }

            while (!queue.IsEmpty())
            {
                var minNode = queue.RemoveMin();
                nodes.Remove(minNode.Data);
                var (u, distu) = (minNode.Data, minNode.Key);
                /*if (distu!=1000000)
                    Console.WriteLine($"queueSize={queue.Size()} current_node={u} dist={distu}");*/
                foreach (var n in GetNeighbors(u).Where(x => nodes.ContainsKey(x)))
                {
                    var d = distu.Add(GetCost(u, n));
                    if (d.CompareTo(costs[n]) < 0) // this one is better
                    {
                        costs[n] = d;
                        prevs[n] = Holder<T>.From(u);
                        queue.DecreaseKey(nodes[n], d);
                    }
                }

            }
        }
    }
}
