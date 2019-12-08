using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day8
{

    public class Layer
    {
        public Layer(char[] data, int width, int height)
        {
            Data = data;
            Width = width;
            Height = height;
        }

        public char[] Data { get; }
        public int Width { get; }
        public int Height { get; }

        public int Count(char c)=>Data.Count(x=>x==c);

        public string Render()
        {
            var span=Data.AsSpan();
            int i=0;
            var builder=new StringBuilder();
            var sp=Width*Height;
            while (i<sp)
            {
                builder.AppendLine(span.Slice(i,Width).ToString());
                i+=Width;
            }
            return builder.ToString();
        }
    }

    public class Image
    {
        public Image(char[] data, int width, int height)
        {
            var span=data.AsSpan();
            int layerLength=width*height;
            for(int i=0;i<data.Length/layerLength;i++)
                Layers.Add(new Layer(span.Slice(layerLength*i,layerLength).ToArray(),width,height));
            Width = width;
            Height = height;
        }

        public IList<Layer> Layers {get;set;}=new List<Layer>();
        public int Width { get; }
        public int Height { get; }

        public Layer FindBestLayer()=>Layers.OrderBy(x=>x.Count('0')).First();

        public Layer Combine()
        {
            var current=new string(' ',Width*Height).ToCharArray();
            foreach(var layer in Layers.Reverse())
            {
                for(int i=0;i<current.Length;i++) {
                    current[i]=Combine(current[i],layer.Data[i]);
                }
            }
            return new Layer(current,Width,Height);
        }

        public static char Combine(char backPixel, char frontPixel)
        {
            if (frontPixel=='2') 
                return backPixel;
            return frontPixel;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var txt=System.IO.File.ReadAllText(args[0]);
            var image=new Image(txt.ToCharArray(),25,6);
            var bestLayer=image.FindBestLayer();
            Console.WriteLine($"1 digit x 2 digit = {bestLayer.Count('1')*bestLayer.Count('2')}");

            var combined=image.Combine();
            Console.WriteLine(combined.Render().Replace('0',' '));

        }
    }
}
