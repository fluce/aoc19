using System;
using IntCode;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Day19
{
    public class SectionMap
    {
        public SectionMap(int width, int height)
        {
            Width = width;
            Height = height;
            Data=new int[Width,Height];
            Clear();
        }

        public void ForEach(Action<int,int,int> action, Action<int> afterLine=null, int step=1, int? fromX=null, int? fromY=null, int? toX=null, int? toY=null)
            => ForEach((x,y,c)=>{ action(x,y,c); return null;}, afterLine, false, step, fromX, fromY, toX, toY).ToList();

        public IEnumerable<(int x,int y,int c)> ForEach()
            => ForEach((x,y,c)=>null, null, true);


        public IEnumerable<(int x,int y,int c)> ForEach(Func<int,int,int,int?> action, Action<int> afterLine=null, bool enumResult=false, int step=1, int? fromX=null, int? fromY=null, int? toX=null, int? toY=null)
        {
            for(int j=fromY??0;j<(toY??Height);j+=step) {
                for(int i=fromX??0;i<(toX??Width);i+=step)
                {
                    var c=action(i,j,Data[i,j]);
                    if (c!=null) Data[i,j]=c.Value;
                    if (enumResult)
                        yield return (i,j,Data[i,j]);
                }
                afterLine?.Invoke(j);
            }
        }

        public IEnumerable<(int x,int y,int c)> ForEach(Func<int,int,int,bool> filter)
        {
            for(int j=0;j<Height;j++) {
                for(int i=0;i<Width;i++)
                {
                    if (filter(i,j,Data[i,j]))
                        yield return (i,j,Data[i,j]);
                }
            }
        }

        public string Dump(int step, int fromX=0, int fromY=0, int? width=null, int? height=null)
        {
            var toX=Math.Min(fromX+width??Width,Width);
            var toY=Math.Min(fromY+height??Height,Height);
            var s=new StringBuilder();
            s.Append("\n");
            ForEach((i,j,c)=>s.Append(ConvertTile(Data[i,j])), _=>s.Append("\n"),step,fromX,fromY,toX,toY);
            return s.ToString();
        }

        public void Clear()=>ForEach((i,j,c)=>0);

        //public char ConvertTile(int tile) => tile switch { 0 => ' ', 1=> '█', 2=> '▒', 3=>'━', 4=>'●', _ => ' ' };
        public char ConvertTile(int tile) => tile switch { 0 => ' ', 1=> '#', 2=> 'O', 3=>'.', 4=>'o', 5=>'?', _ => 'O' };
        public int[,] Data { get; private set;}

        public int Width { get; }
        public int Height { get; }
    }


    class Drone
    {
        public SectionMap map=new SectionMap(10000,10000);

        public IntCodeComputer Computer {get;set;}
        public Drone(string program)
        {
            Computer=new IntCodeComputer(program);
            Computer.SetOutput = OutputHandler;
            Computer.GetNextInput = InputHandler;
            //Computer.Log = Console.WriteLine;
        }

        public void Run(int startX, int startY, int scanStep, int scanWidth)
        {
            lastX=startX;
            lastY=startY;
            this.startX=startX;
            this.startY=startY;
            stopX=startX+scanWidth;
            stopY=startY+scanWidth;
            step=scanStep;
            theEnd=false;
            InputQueue.Add(startX);
            InputQueue.Add(startY);
            while (!theEnd) {
                Computer.Reset();
                Computer.Execute();
                //Console.WriteLine(map.Dump(scanStep,startX,startY,scanWidth));
            }
        }

        int step;
        int startX;
        int startY;
        int lastX=0;
        int lastY=0;
        int stopX;
        int stopY;
        bool theEnd=false;
        //public BlockingCollection<long> OutputQueue=new BlockingCollection<long>(new ConcurrentQueue<long>());
        public void OutputHandler(long? c)
        {
            map.Data[lastX,lastY]=(int)c.Value;
            //Console.WriteLine($"Scanned {lastX},{lastY} => {c}");
            lastX+=step;
            if (lastX>=stopX) {
                lastX=startX;
                //Console.WriteLine(map.Dump(1,0,lastY,null,1));
                lastY+=step;
            }
            if (lastY<stopY) {
                InputQueue.Add(lastX);
                InputQueue.Add(lastY);
            } else {
                theEnd=true;
            }
        }

        public BlockingCollection<long> InputQueue=new BlockingCollection<long>(new ConcurrentQueue<long>());

        public long InputHandler()
        {
            if (InputQueue.TryTake(out var item, 10000))
                return item;
            throw new Exception("No more input");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string input="109,424,203,1,21102,1,11,0,1106,0,282,21101,0,18,0,1105,1,259,1201,1,0,221,203,1,21102,31,1,0,1105,1,282,21101,38,0,0,1106,0,259,20101,0,23,2,22102,1,1,3,21101,0,1,1,21101,0,57,0,1106,0,303,2101,0,1,222,21001,221,0,3,20102,1,221,2,21102,1,259,1,21102,1,80,0,1106,0,225,21101,33,0,2,21102,1,91,0,1106,0,303,1201,1,0,223,21002,222,1,4,21101,259,0,3,21101,0,225,2,21101,225,0,1,21101,0,118,0,1106,0,225,20101,0,222,3,21102,1,102,2,21102,133,1,0,1105,1,303,21202,1,-1,1,22001,223,1,1,21101,148,0,0,1106,0,259,2101,0,1,223,21001,221,0,4,21002,222,1,3,21101,0,15,2,1001,132,-2,224,1002,224,2,224,1001,224,3,224,1002,132,-1,132,1,224,132,224,21001,224,1,1,21102,195,1,0,106,0,108,20207,1,223,2,21001,23,0,1,21102,1,-1,3,21101,0,214,0,1105,1,303,22101,1,1,1,204,1,99,0,0,0,0,109,5,2102,1,-4,249,22101,0,-3,1,22101,0,-2,2,21202,-1,1,3,21101,250,0,0,1105,1,225,22102,1,1,-4,109,-5,2106,0,0,109,3,22107,0,-2,-1,21202,-1,2,-1,21201,-1,-1,-1,22202,-1,-2,-2,109,-3,2105,1,0,109,3,21207,-2,0,-1,1206,-1,294,104,0,99,22101,0,-2,-2,109,-3,2106,0,0,109,5,22207,-3,-4,-1,1206,-1,346,22201,-4,-3,-4,21202,-3,-1,-1,22201,-4,-1,2,21202,2,-1,-1,22201,-4,-1,1,22101,0,-2,3,21102,1,343,0,1106,0,303,1106,0,415,22207,-2,-3,-1,1206,-1,387,22201,-3,-2,-3,21202,-2,-1,-1,22201,-3,-1,3,21202,3,-1,-1,22201,-3,-1,2,22102,1,-4,1,21102,384,1,0,1106,0,303,1106,0,415,21202,-4,-1,-4,22201,-4,-3,-4,22202,-3,-2,-2,22202,-2,-4,-4,22202,-3,-2,-3,21202,-4,-1,-2,22201,-3,-2,1,21202,1,1,-4,109,-5,2106,0,0";

            var c=new Drone(input);

            int startScanX=0;
            int startScanY=0;
            int iter=0;
            int resolution=48;
            int scanDist=96;
            int scanWidth=10000-Math.Max(startScanX,startScanY);
            while (true)
                {
                    c.Run(startScanX,startScanY,resolution,scanWidth);
                    var r=c.map.ForEach((x,y,d)=>{                        
                        if (d==1 && x<c.map.Width-100 && y<c.map.Height-100 && 
                            /*c.map.Data[x+100,y+100]==1 &&*/ c.map.Data[x+scanDist,y]==1 && c.map.Data[x,y+scanDist]==1)
                        {
                            Console.WriteLine($"Found ! {x},{y} => {x*10000+y}");
                            //Console.WriteLine(c.map.Dump(resolution,startScanX,startScanY,scanWidth));
                            startScanX=x-50;
                            startScanY=y-50;
                            scanWidth=400;
                            scanDist=99;
                            resolution=1;
                            iter++;
                            return true;
                        }
                        return false;
                    }).FirstOrDefault();                    
                    if (r.c==1) {
                        //c.Run(startScanX,startScanY,resolution, scanWidth);
                        Console.WriteLine($"Scan {startScanX},{startScanY} - {scanWidth}/{resolution}");
                        //Console.WriteLine(c.map.Dump(resolution,startScanX,startScanY,scanWidth));
                        if (iter>6) break;

                    } else break;
                }
            
            //Console.WriteLine($"Part 1 : {c.map.ForEach((x,y,c)=>c==1).Count()}");

        }
    }
}
