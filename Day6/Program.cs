using System;
using System.Collections.Generic;
using System.Linq;

namespace Day6
{
    public class SpatialObject
    {
        public SpatialObject OrbitsAround {get;set;}
        public string Name {get;set;}
        public ICollection<SpatialObject> Satellites {get;} = new List<SpatialObject>();
        public int Color {get;set;}
    }
    public class InputParser
    {        
        public InputParser(params string[] input)
        {
            Input=input;
        }
        public string[] Input {get;private set;}

        public IEnumerable<SpatialObject> Roots {get;private set;}
        public IDictionary<string,SpatialObject> AllObjects {get;private set;}
        public void Parse()
        {                
            Dictionary<string,SpatialObject> objectIndex=new Dictionary<string, SpatialObject>();

            foreach(var iterSpatialObject in Input.Select(ParseOne))
            {
                var spatialObject=iterSpatialObject;
                if (objectIndex.TryGetValue(spatialObject.Name, out var existingSpatialObject)) 
                {
                    //throw new InvalidOperationException($"SpatialObject {spatialObject.Name} already exists");
                    if (existingSpatialObject.OrbitsAround!=null)
                        throw new InvalidOperationException($"SpatialObject {spatialObject.Name} already has a parent : {existingSpatialObject.OrbitsAround.Name}");
                    existingSpatialObject.OrbitsAround=spatialObject.OrbitsAround;
                    existingSpatialObject.OrbitsAround.Satellites.Clear();
                    existingSpatialObject.OrbitsAround.Satellites.Add(existingSpatialObject);
                    spatialObject=existingSpatialObject;
                } else {
                    objectIndex.Add(spatialObject.Name,spatialObject);
                }

                if (objectIndex.TryGetValue(spatialObject.OrbitsAround.Name, out var orbitsAround))  // parent found
                {
                    spatialObject.OrbitsAround=orbitsAround;
                    orbitsAround.Satellites.Add(spatialObject);
                } else { // parent not found
                    objectIndex.Add(spatialObject.OrbitsAround.Name,spatialObject.OrbitsAround);
                }

            }
            Roots=objectIndex.Values.Where(x=>x.OrbitsAround==null).ToList();
            AllObjects=objectIndex;

        }

        public static SpatialObject ParseOne(string input)
        {
            var r=input.Split(')');
            var n=new SpatialObject {Name=r[1], OrbitsAround=new SpatialObject{Name=r[0]}};
            n.OrbitsAround.Satellites.Add(n);
            return n;
        }

        public static int CalculateDirectAndIndirectOrbits(SpatialObject obj)
        {
            int n=0;
            while (obj.OrbitsAround!=null) { n++; obj=obj.OrbitsAround; }
            return n;
        }

        public static void Dump(SpatialObject obj, Action<string> write, string level="")
        {
            write($"{level}{obj.Name} - {obj.Color}");
            foreach(var s in obj.Satellites)
                Dump(s,write,level+"|  ");
        }

        public static IEnumerable<SpatialObject> GetPathToRootSpatialObject(SpatialObject obj)        
        {
            obj=obj.OrbitsAround;
            while (obj!=null){
                yield return obj;
                obj=obj.OrbitsAround;
            }
        }

        public (bool,int) CalculateOrbitalTransfersCount(SpatialObject from, SpatialObject to, Action<string> write)
        {
            foreach(var o in AllObjects.Values) o.Color=0;

            var fromPath=GetPathToRootSpatialObject(from);
            var root=fromPath.Last();
            foreach(var o in fromPath) o.Color=1;

            Dump(root,write);

            var toPath=GetPathToRootSpatialObject(to);
            var tillIntersection1=toPath.TakeWhile(x=>x.Color==0);
            if (tillIntersection1.Count()==0) // Cas dégénéré, to déjà sur le chemin vers la racine depuis from
            {
                write("Cas dégénéré");
                var tillIntersection2=fromPath.TakeWhile(x=>x!=toPath.FirstOrDefault());
                foreach(var o in tillIntersection2) o.Color=3;
                Dump(root,write);
                return (true,tillIntersection2.Count());
            } else {
                var intersection=tillIntersection1.Last().OrbitsAround.Color=2;
                Dump(root,write);
                var tillIntersection2=fromPath.TakeWhile(x=>x.Color!=2);
                foreach(var o in tillIntersection2) o.Color=4;
                Dump(root,write);
                return (false,tillIntersection1.Count()+tillIntersection2.Count());
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var parser=new InputParser(System.IO.File.ReadAllLines(args[0]));
            parser.Parse();
            foreach(var r in parser.Roots) InputParser.Dump(r,Console.WriteLine);

            var result=parser.AllObjects.Values.Sum(InputParser.CalculateDirectAndIndirectOrbits);

            var (degenerative,n)=parser.CalculateOrbitalTransfersCount(parser.AllObjects["YOU"],parser.AllObjects["SAN"], Console.WriteLine);
            if (degenerative) Console.WriteLine("Degenerative case");

            Console.WriteLine($"Total orbits : {result}");
            Console.WriteLine($"Total transfers : {n}");
            

        }
    }
}
