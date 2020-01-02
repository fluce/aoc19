using System;
using System.Linq;
using System.Collections.Generic;
using IntCode;

namespace Day17
{
    public class Screen
    {
        private int state=0;
        private List<char> buffer=new List<char>();
        private List<char[]> screen=new List<char[]>();
        public int Width {get;set;}=0;
        public int Height => screen.Count;
        public void UpdateScreen(long? data)
        {
            if (data>255)
                Console.WriteLine("Dust collected : "+data);
            else {
                if (state==1) {
                    Console.WriteLine(data);
                    Console.WriteLine(Dump());
                    buffer.Clear();
                    screen.Clear();
                    state=0;
                }
                if (state==0) {
                    if (data.Value==10) {
                        if (Width==0) { Width=buffer.Count; };
                        if (buffer.Count>0) screen.Add(buffer.ToArray());
                            else state=1;
                        buffer.Clear();
                    }
                    else {
                        buffer.Add(Convert.ToChar(data.Value));
                    }

                }
            }
        }

        public string Dump() {
            return string.Join(Environment.NewLine,screen.Select(x=>string.Join(' ',x)));
        }

        public char this[int x, int y] {
            get {
                if (x<0 || x>=Width) return ' ';
                if (y<0 || y>=Height) return ' ';
                return screen[y][x];
            }
        }

        public IEnumerable<(Screen s,int x,int y, char v)> ForEach(Func<Screen,int,int,char,bool> filter=null) 
        {
            for (int x=0;x<Width;x++)
                for (int y=0;y<Height;y++)
                    if (filter?.Invoke(this,x,y,this[x,y])??true) yield return (this,x,y,this[x,y]);

        }
    }

    public class ASCIIComputer
    {
        public IntCodeComputer Computer { get;set; }
        public Screen Screen {get;set;}

        private int[] Prg;

        public ASCIIComputer(string program, string p2)
        {
            Computer=new IntCodeComputer(program);
            
            Screen=new Screen();
            Computer.SetOutput = Screen.UpdateScreen;
            Computer.GetNextInput = IntCodeComputer.GetInputsFrom(ConvertPrg(p2));
        }

        public static long[] ConvertPrg(string p2)
        {
            var l=new List<long>();
            foreach(var s in p2.Split(new char[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)) {
                l.AddRange(s.Select(Convert.ToInt64));
                l.Add(10);
            }
            return l.ToArray();
        }

        public IEnumerable<(int x,int y)> GetIntersections()
        {
            return Screen.ForEach((screen,x,y,s)=>s=='#').Where(i=>Screen[i.x-1,i.y]=='#' && Screen[i.x+1,i.y]=='#' && Screen[i.x,i.y-1]=='#'&&Screen[i.x,i.y+1]=='#').Select(i=>(i.x,i.y));
        }

        public int GetSum()=>GetIntersections().Sum(i=>i.x*i.y);

        (int dx,int dy,int idx)[] directions={(0,1,0),(-1,0,1),(0,-1,2),(1,0,3)};
        string dirChar="v<^>";
        char[] CalcTurn((int dx,int dy,int idx) currentDirection, (int dx,int dy,int idx) dirToGo) {
            return (currentDirection.dx*dirToGo.dy-currentDirection.dy*dirToGo.dx) switch { -1=>new char[] { 'L' }, 1=>new char[] { 'R' },_=>new char[] {} };
        }

        public class Step {}
        public class TurnStep:Step { public char Direction {get;set;} public override string ToString() => Direction.ToString(); }
        public class ForwardStep:Step { public int Steps {get;set;} public override string ToString() => Steps.ToString(); }

        public List<Step> FindPath()
        {
            var ret=new List<Step>();
            var initpos=Screen.ForEach((Screen,x,y,s)=>dirChar.Contains(s)).FirstOrDefault();
            var currentDirection=directions[dirChar.IndexOf(initpos.v)];
            var curpos=(initpos.x, initpos.y);
            var dirToGo=directions.FirstOrDefault(d=>Screen[curpos.x+d.dx,curpos.y+d.dy]=='#');
            Console.WriteLine($"curpos={curpos} currentDirection={currentDirection} dirToGo={dirToGo}");
            while (!(dirToGo.dx==0 && dirToGo.dy==0)) 
            {
                ret.AddRange(CalcTurn(currentDirection,dirToGo).Select(x=>new TurnStep { Direction=x })); 
                currentDirection=dirToGo;
                int stepInCurrentDirection=0;
                do { stepInCurrentDirection++; } while (Screen[curpos.x+stepInCurrentDirection*currentDirection.dx,curpos.y+stepInCurrentDirection*currentDirection.dy]=='#');
                stepInCurrentDirection--;
                Console.WriteLine($"stepInCurrentDirection={stepInCurrentDirection}");
                ret.Add(new ForwardStep { Steps = stepInCurrentDirection });
                curpos=(curpos.x+stepInCurrentDirection*currentDirection.dx,curpos.y+stepInCurrentDirection*currentDirection.dy);
                dirToGo=directions.FirstOrDefault(d=>(Math.Abs(dirToGo.idx-d.idx)%2)==1&&Screen[curpos.x+d.dx,curpos.y+d.dy]=='#');
                Console.WriteLine($"curpos={curpos} currentDirection={currentDirection} dirToGo={dirToGo}");
            }
            return ret;
        }

        public void FindPatterns(List<Step> steps, int size, Dictionary<(string key,int size),List<int>> indices)
        {
            for (int i=0;i<=steps.Count-size;i+=2)
            {
                var pattern=steps.Skip(i).Take(size).ToArray();
                var patternKey=string.Join(',',pattern.Select(x=>x.ToString()));
                if (patternKey.Length<=20) {
                    if (indices.TryGetValue((patternKey,size),out var l)) 
                        l.Add(i);
                    else
                        indices[(patternKey,size)]=new List<int> { i };
                    //Console.WriteLine($"{i,3} {patternKey} : {string.Join(',',indices[patternKey].Select(x=>x.ToString()))}");
                }
            }
        }

        public string TryCombinations(List<Step> steps, Dictionary<(string key,int size),List<int>> indices)
        {
            int size=steps.Count;
            var a=new List<(int c_plein,string key,int size,List<int> list, string schema)>();
            var r=new char[size];
            foreach(var s in indices.Where(x=>x.Value.Count>1).OrderByDescending(x=>x.Value.Count*x.Key.size)) {
                Array.Fill(r,' ');
                foreach(var a2 in s.Value) Array.Fill(r,'=',a2,s.Key.size);

                var c_plein=r.Count(x=>x!=' ');
                a.Add((c_plein,s.Key.key, s.Key.size, s.Value,new string(r)));
                //Console.WriteLine(string.Join(" ",dico.Select(x=>$"{x.Key}={x.Value}")));
                Console.WriteLine($"{s.Key.key,20} = [{new string(r)}]  n_plein={c_plein,2}/{steps.Count} {string.Join(',',s.Value.Select(x=>x.ToString()))}");
            }

            foreach(var combination in Combinations.CombinationsRosettaWoRecursion(a.ToArray(),3))
            {
                if (combination.Sum(x=>x.c_plein)==size) {
                    Console.WriteLine($"Combinaisons : ");
                    Array.Fill(r,' ');
                    foreach(var a2 in combination[0].list) Array.Fill(r,'A',a2,combination[0].size);
                    foreach(var a2 in combination[1].list) Array.Fill(r,'B',a2,combination[1].size);
                    foreach(var a2 in combination[2].list) Array.Fill(r,'C',a2,combination[2].size);
                    foreach(var i in combination) {
                        Console.WriteLine($"{i.key,20} = [{i.schema}]  n_plein={i.c_plein,2}/{steps.Count} {string.Join(',',i.list.Select(x=>x.ToString()))}");
                    }
                    Console.WriteLine($"{"",20} = [{new string(r)}]");
                    if (r.All(x=>x!=' ')) {
                        Console.WriteLine("Found !!");
                        var seq=string.Join(',',combination.SelectMany(x=>x.list).OrderBy(x=>x).Select(x=>r[x].ToString()));
                        return seq+Environment.NewLine+combination[0].key+Environment.NewLine+combination[1].key+Environment.NewLine+combination[2].key;
                    }

                }
            }
            return "";
        }

        public void Run(int mode=1)
        {
            Computer.Memory[0]=mode;
            Computer.Execute();
            Console.WriteLine(Screen.Dump());
            if (mode==1)
            //foreach(var (x,y) in GetIntersections()) Console.WriteLine($"x={x}, y={y}");
                Console.WriteLine($"Sum = {GetSum()}");
        }
    }

    static class Combinations
    {
        // Enumerate all possible m-size combinations of [0, 1, ..., n-1] array
        // in lexicographic order (first [0, 1, 2, ..., m-1]).
        private static IEnumerable<int[]> CombinationsRosettaWoRecursion(int m, int n)
        {
            int[] result = new int[m];
            Stack<int> stack = new Stack<int>(m);
            stack.Push(0);
            while (stack.Count > 0)
            {
                int index = stack.Count - 1;
                int value = stack.Pop();
                while (value < n)
                {
                    result[index++] = value++;
                    stack.Push(value);
                    if (index != m) continue;
                    yield return (int[])result.Clone(); // thanks to @xanatos
                    //yield return result;
                    break;
                }
            }
        }

        public static IEnumerable<T[]> CombinationsRosettaWoRecursion<T>(T[] array, int m)
        {
            if (array.Length < m)
                throw new ArgumentException("Array length can't be less than number of selected elements");
            if (m < 1)
                throw new ArgumentException("Number of selected elements can't be less than 1");
            T[] result = new T[m];
            foreach (int[] j in CombinationsRosettaWoRecursion(m, array.Length))
            {
                for (int i = 0; i < m; i++)
                {
                    result[i] = array[j[i]];
                }
                yield return result;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)        
        {
            var program="1,330,331,332,109,3524,1102,1182,1,16,1102,1457,1,24,102,1,0,570,1006,570,36,1001,571,0,0,1001,570,-1,570,1001,24,1,24,1105,1,18,1008,571,0,571,1001,16,1,16,1008,16,1457,570,1006,570,14,21101,0,58,0,1106,0,786,1006,332,62,99,21102,1,333,1,21102,1,73,0,1105,1,579,1102,1,0,572,1101,0,0,573,3,574,101,1,573,573,1007,574,65,570,1005,570,151,107,67,574,570,1005,570,151,1001,574,-64,574,1002,574,-1,574,1001,572,1,572,1007,572,11,570,1006,570,165,101,1182,572,127,102,1,574,0,3,574,101,1,573,573,1008,574,10,570,1005,570,189,1008,574,44,570,1006,570,158,1106,0,81,21102,1,340,1,1105,1,177,21102,477,1,1,1105,1,177,21101,0,514,1,21101,0,176,0,1106,0,579,99,21101,184,0,0,1106,0,579,4,574,104,10,99,1007,573,22,570,1006,570,165,1002,572,1,1182,21101,375,0,1,21102,1,211,0,1106,0,579,21101,1182,11,1,21102,1,222,0,1105,1,979,21101,388,0,1,21102,233,1,0,1105,1,579,21101,1182,22,1,21102,1,244,0,1105,1,979,21101,0,401,1,21101,0,255,0,1105,1,579,21101,1182,33,1,21101,0,266,0,1105,1,979,21102,414,1,1,21102,277,1,0,1106,0,579,3,575,1008,575,89,570,1008,575,121,575,1,575,570,575,3,574,1008,574,10,570,1006,570,291,104,10,21101,1182,0,1,21102,313,1,0,1106,0,622,1005,575,327,1102,1,1,575,21101,0,327,0,1105,1,786,4,438,99,0,1,1,6,77,97,105,110,58,10,33,10,69,120,112,101,99,116,101,100,32,102,117,110,99,116,105,111,110,32,110,97,109,101,32,98,117,116,32,103,111,116,58,32,0,12,70,117,110,99,116,105,111,110,32,65,58,10,12,70,117,110,99,116,105,111,110,32,66,58,10,12,70,117,110,99,116,105,111,110,32,67,58,10,23,67,111,110,116,105,110,117,111,117,115,32,118,105,100,101,111,32,102,101,101,100,63,10,0,37,10,69,120,112,101,99,116,101,100,32,82,44,32,76,44,32,111,114,32,100,105,115,116,97,110,99,101,32,98,117,116,32,103,111,116,58,32,36,10,69,120,112,101,99,116,101,100,32,99,111,109,109,97,32,111,114,32,110,101,119,108,105,110,101,32,98,117,116,32,103,111,116,58,32,43,10,68,101,102,105,110,105,116,105,111,110,115,32,109,97,121,32,98,101,32,97,116,32,109,111,115,116,32,50,48,32,99,104,97,114,97,99,116,101,114,115,33,10,94,62,118,60,0,1,0,-1,-1,0,1,0,0,0,0,0,0,1,52,26,0,109,4,1201,-3,0,586,21001,0,0,-1,22101,1,-3,-3,21102,0,1,-2,2208,-2,-1,570,1005,570,617,2201,-3,-2,609,4,0,21201,-2,1,-2,1105,1,597,109,-4,2106,0,0,109,5,1201,-4,0,629,21002,0,1,-2,22101,1,-4,-4,21101,0,0,-3,2208,-3,-2,570,1005,570,781,2201,-4,-3,653,20102,1,0,-1,1208,-1,-4,570,1005,570,709,1208,-1,-5,570,1005,570,734,1207,-1,0,570,1005,570,759,1206,-1,774,1001,578,562,684,1,0,576,576,1001,578,566,692,1,0,577,577,21102,1,702,0,1106,0,786,21201,-1,-1,-1,1105,1,676,1001,578,1,578,1008,578,4,570,1006,570,724,1001,578,-4,578,21102,1,731,0,1105,1,786,1105,1,774,1001,578,-1,578,1008,578,-1,570,1006,570,749,1001,578,4,578,21101,0,756,0,1105,1,786,1105,1,774,21202,-1,-11,1,22101,1182,1,1,21101,0,774,0,1106,0,622,21201,-3,1,-3,1105,1,640,109,-5,2105,1,0,109,7,1005,575,802,20102,1,576,-6,20101,0,577,-5,1106,0,814,21101,0,0,-1,21101,0,0,-5,21101,0,0,-6,20208,-6,576,-2,208,-5,577,570,22002,570,-2,-2,21202,-5,53,-3,22201,-6,-3,-3,22101,1457,-3,-3,1201,-3,0,843,1005,0,863,21202,-2,42,-4,22101,46,-4,-4,1206,-2,924,21101,0,1,-1,1106,0,924,1205,-2,873,21102,1,35,-4,1105,1,924,2102,1,-3,878,1008,0,1,570,1006,570,916,1001,374,1,374,1202,-3,1,895,1101,0,2,0,2102,1,-3,902,1001,438,0,438,2202,-6,-5,570,1,570,374,570,1,570,438,438,1001,578,558,922,20101,0,0,-4,1006,575,959,204,-4,22101,1,-6,-6,1208,-6,53,570,1006,570,814,104,10,22101,1,-5,-5,1208,-5,39,570,1006,570,810,104,10,1206,-1,974,99,1206,-1,974,1101,0,1,575,21102,973,1,0,1106,0,786,99,109,-7,2105,1,0,109,6,21102,1,0,-4,21102,1,0,-3,203,-2,22101,1,-3,-3,21208,-2,82,-1,1205,-1,1030,21208,-2,76,-1,1205,-1,1037,21207,-2,48,-1,1205,-1,1124,22107,57,-2,-1,1205,-1,1124,21201,-2,-48,-2,1106,0,1041,21102,1,-4,-2,1105,1,1041,21102,1,-5,-2,21201,-4,1,-4,21207,-4,11,-1,1206,-1,1138,2201,-5,-4,1059,1202,-2,1,0,203,-2,22101,1,-3,-3,21207,-2,48,-1,1205,-1,1107,22107,57,-2,-1,1205,-1,1107,21201,-2,-48,-2,2201,-5,-4,1090,20102,10,0,-1,22201,-2,-1,-2,2201,-5,-4,1103,2102,1,-2,0,1106,0,1060,21208,-2,10,-1,1205,-1,1162,21208,-2,44,-1,1206,-1,1131,1105,1,989,21101,439,0,1,1106,0,1150,21101,477,0,1,1106,0,1150,21101,0,514,1,21102,1149,1,0,1105,1,579,99,21102,1,1157,0,1105,1,579,204,-2,104,10,99,21207,-3,22,-1,1206,-1,1138,1201,-5,0,1176,2102,1,-4,0,109,-6,2106,0,0,14,11,1,11,30,1,9,1,1,1,9,1,30,1,7,9,5,1,30,1,7,1,1,1,1,1,3,1,5,1,30,1,7,1,1,1,1,1,3,1,5,1,30,1,7,1,1,1,1,1,3,1,5,1,30,1,1,9,1,1,3,1,5,1,30,1,1,1,5,1,3,1,3,1,5,1,30,1,1,1,5,1,3,9,1,1,30,1,1,1,5,1,7,1,3,1,1,1,30,9,7,7,32,1,17,1,34,1,11,9,32,1,11,1,5,1,1,1,32,9,3,1,5,1,1,1,40,1,3,1,5,1,1,1,40,1,3,1,5,1,1,1,40,1,3,1,7,1,40,1,3,1,7,7,34,1,3,1,13,1,30,9,13,1,30,1,3,1,17,1,28,7,17,1,28,1,1,1,21,1,28,1,1,1,21,1,28,1,1,1,21,1,28,1,1,1,21,11,18,1,1,1,32,7,3,9,1,1,32,1,5,1,3,1,9,1,32,1,5,1,3,1,1,9,32,1,5,1,3,1,1,1,40,1,5,1,3,1,1,1,40,1,5,1,3,1,1,1,40,1,5,1,3,1,1,1,40,1,5,1,3,1,1,1,40,1,5,7,40,1,9,1,42,11,42";
            var c=new ASCIIComputer(program,"");
            c.Run();

            var path=c.FindPath();
            Console.WriteLine("Path = "+string.Join(',',path.Select(x=>x.ToString())));
            var indices=new Dictionary<(string key,int size), List<int>>();

            c.FindPatterns(path,4,indices);
            c.FindPatterns(path,6,indices);
            c.FindPatterns(path,8,indices);
            c.FindPatterns(path,10,indices);

            var prg=c.TryCombinations(path,indices);
            Console.WriteLine("Program :");
            Console.WriteLine(prg);

            c=new ASCIIComputer(program,prg+Environment.NewLine+"n");
            c.Run(2);
        }
    }
}
