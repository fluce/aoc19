using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public class Table<T>
    {
        private T[] Data {get;set;}
        public int Width {get; private set;}
        public int Height {get; private set;}
        public Table()
        {
            /*Data=data;
            Width=data[0].Length;
            Height=data.Length;*/
        }

        public Func<(int x,int y),T> OutsideValue {get;set;} = p=>default(T);
        public static Table<T> Parse(string[] data, Func<(int x,int y), char, T> convertor) 
        {
            var t=new Table<T>() {
                Width=data[0].Length,
                Height=data.Length,
            };
            t.Data=data.Index().SelectMany(i=>i.Item.Index().Select(j=>convertor((j.Idx,i.Idx),j.Item))).ToArray();
            return t;
        }

        public static Table<T> Parse(string data, Func<(int x,int y), char, T> convertor) => Parse(data.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries), convertor);

        public T this[(int x,int y) c] {
            get { return this[c.x,c.y]; }
            set { this[c.x,c.y]=value; }
        }

        public T this[int x,int y] {
            get {
                if (x<0 || y<0 || x>=Width || y>=Height)
                    return OutsideValue((x,y));
                return Data[y*Width+x];
            }
            set {
                if (x>=0 && y>=0 && x<Width && y<Height)
                    Data[y*Width+x]=value;
            }
        }

        public IEnumerable<(int X, int Y, T Item)> Each(Func<int,int,T,bool> predicate=null)
        {
            for (int j=0;j<Height;j++)
                for (int i=0;i<Width;i++)
                    if (predicate?.Invoke(i,j,this[i,j])??true)
                        yield return (i,j,this[i,j]);
        }

        public IEnumerable<((int X,int Y) Position, T Item)> Each(Func<(int X,int Y),T,bool> predicate=null)
        {
            for (int j=0;j<Height;j++)
                for (int i=0;i<Width;i++)
                    if (predicate?.Invoke((i,j),this[i,j])??true)
                        yield return ((i,j),this[i,j]);
        }

        public string Dump(Func<(int X,int Y),T,string> convertor=null)
        {
            var buf=new StringBuilder();
            for (int j=0;j<Height;j++) 
            {
                for (int i=0;i<Width;i++)
                    buf.Append((convertor??Convertor)((i,j),this[i,j]));
                buf.AppendLine();
            }
            return buf.ToString();
        }

        protected virtual string Convertor((int x,int y) p, T v)
        {
            return v.ToString();
        }

    }
}
