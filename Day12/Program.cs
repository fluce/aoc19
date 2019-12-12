using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace Day12
{
    public class Moon
    {
        static int _id=0;
        public int Id = _id++;
        public int X;
        public int Y;
        public int Z;

        public int vX;
        public int vY;
        public int vZ;

        static readonly Regex regex=new Regex(@"(pos=)?\<x=\s*(?<x>[0-9\-]+), y=\s*(?<y>[0-9\-]+), z=\s*(?<z>[0-9\-]+)\>(, vel=\<x=\s*(?<vx>[0-9\-]+), y=\s*(?<vy>[0-9\-]+), z=\s*(?<vz>[0-9\-]+)\>)?");

        public static Moon ParseInput(string line)
        {
            var result=regex.Match(line);
            return new Moon { 
                X=int.Parse(result.Groups["x"].Value), 
                Y=int.Parse(result.Groups["y"].Value), 
                Z=int.Parse(result.Groups["z"].Value),
                vX = result.Groups["vx"].Length == 0 ? 0 : int.Parse(result.Groups["vx"].Value),
                vY = result.Groups["vy"].Length == 0 ? 0 : int.Parse(result.Groups["vy"].Value),
                vZ = result.Groups["vz"].Length == 0 ? 0 : int.Parse(result.Groups["vz"].Value),
            };
        }

        public override bool Equals(object obj) 
        {
            var o=obj as Moon;
            if (o==null) return false;
            return o.X==X && o.Y==Y && o.Z==Z && o.vX==vX && o.vY==vY && o.vZ==vZ;
        }
        public override string ToString()
        {
            return $"<x={X}, y={Y}, z={Z}> <vx={vX}, vy={vY}, vz={vZ}>";
        }

    }

    public class JupiterSystem
    {
        public List<Moon> Moons {get;} = new List<Moon>();

        public JupiterSystem(string input)
        {
/*            
<x=-8, y=-10, z=0>
<x=5, y=5, z=10>
<x=2, y=-7, z=3>
<x=9, y=-8, z=-3>
*/
            Moons.AddRange(input.Split(new[]{'\n','\r'}, StringSplitOptions.RemoveEmptyEntries).Select(Moon.ParseInput));
        }

        public IEnumerable<(Moon,Moon)> GetPairs()
        {
            return Moons.SelectMany(x=>Moons.Select(y=>(x,y)).Where(p=>p.x.Id>p.y.Id));
        }

        public static void ApplyGravity(Moon m1, Moon m2)
        {
            ApplyGravity(m1.X,m2.X,ref m1.vX, ref m2.vX);
            ApplyGravity(m1.Y,m2.Y,ref m1.vY, ref m2.vY);
            ApplyGravity(m1.Z,m2.Z,ref m1.vZ, ref m2.vZ);
        }

        public static void ApplyGravity(int p1, int p2, ref int v1, ref int v2)
        {
            if (p1>p2) { v1--; v2++; } else if (p2>p1) { v1++; v2--; }
        }

        public void ApplyVelocity(Moon m)
        {
            m.X+=m.vX;
            m.Y+=m.vY;
            m.Z+=m.vZ;
        }

        public void SimulateTimeStep()
        {
            foreach (var (m1,m2) in GetPairs()) ApplyGravity(m1,m2);
            foreach (var m in Moons) ApplyVelocity(m);
        }

        public void SimulateTimeStep(int n)
        {
            for(var i=0;i<n;i++)
                SimulateTimeStep();
        }

        public int GetTotalEnergy()
        {
            return Moons.Sum(m=>(Math.Abs(m.X)+Math.Abs(m.Y)+Math.Abs(m.Z))*(Math.Abs(m.vX)+Math.Abs(m.vY)+Math.Abs(m.vZ)));
        }

        MD5 md5=MD5.Create();
        public byte[] GetUniverseHash(Func<Moon,string> selector)
        {
            return md5.ComputeHash(Encoding.ASCII.GetBytes(string.Join(',', Moons.Select(selector))));
            //return Moons[0].X;
        }
    }

    class Program
    {

        static int GetPeriod(string program, Func<Moon,string> selector)
        {
            var system=new JupiterSystem(program);
/*@"<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>");           */
/*            var system=new JupiterSystem(@"<x=-8, y=-10, z=0>
<x=5, y=5, z=10>
<x=2, y=-7, z=3>
<x=9, y=-8, z=-3>");*/

/*            var system=new JupiterSystem(@"<x=0, y=6, z=1>
<x=4, y=4, z=19>
<x=-11, y=1, z=8>
<x=2, y=19, z=15>");*/

            byte[] initHash=system.GetUniverseHash(selector);
            //Console.WriteLine(system.Moons[0]);
            int i=0;
            while (true)
            {
                system.SimulateTimeStep();
                //Console.WriteLine(system.Moons[0]);
                var hash=system.GetUniverseHash(selector);
                if (initHash.SequenceEqual(hash)) {
                    Console.WriteLine($"Old Pos ! {i} { system.Moons[0].ToString() }");  
                    return i+1;                  
                }
                i++;
            }

        }


        static long LCM(params long[] numbers)
        {
            return numbers.Aggregate(lcm);
        }
        static long lcm(long a, long b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }
        static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        static void Main(string[] args)
        {
            string program=@"<x=0, y=6, z=1>
<x=4, y=4, z=19>
<x=-11, y=1, z=8>
<x=2, y=19, z=15>";
            var system=new JupiterSystem(program);
            system.SimulateTimeStep(1000);
            Console.WriteLine($"Energy = {system.GetTotalEnergy()}");

            Console.WriteLine("=> X");
            var periodX=GetPeriod(program,x=>x.X+" "+x.vX); //+" "+x.Y+" "+x.vY+" "+x.Z+" "+x.vZ);
            Console.WriteLine("=> Y");
            var periodY=GetPeriod(program,x=>x.Y+" "+x.vY);
            Console.WriteLine("=> Z");
            var periodZ=GetPeriod(program,x=>x.Z+" "+x.vZ);
            Console.WriteLine($"GlobalPeriod {LCM(periodX,periodY,periodZ)}");
        }
    }
}
