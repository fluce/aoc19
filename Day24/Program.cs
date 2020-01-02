using System;
using System.Linq;
using System.Collections.Generic;
using Utils;

namespace Day24
{
    public class LifeGame
    {
        public class Item {
            public bool HasBug {get;set;}
            public int? AdjacentCount {get;set;}
        }

        public Dictionary<int,Table<Item>> Data {get;set;}

        readonly (int dx,int dy)[] directions = new (int dx,int dy)[] { (0,-1),(1,0),(0,1),(-1,0) };

        public int BioDiversityRating=>Data[0].Each((p,i)=>true)
                                           .Index()
                                           .Where(x=>x.Item.Item.HasBug)
                                           .Select(x=>x.Idx)
                                           .Sum(x=>1<<x);

        public int BugCount=>Data.Sum(x=>x.Value.Each((p,i)=>i.HasBug).Count());

        public LifeGame(string input)
        {
            Data=new Dictionary<int, Table<Item>>();
            Data[0]=Table<Item>.Parse(input,(p,c)=>new Item {HasBug=c=='#'});
            for(int i=1;i<=200;i++)
            {
                Data[i]=Table<Item>.Parse(input,(p,c)=>new Item {HasBug=false});
                Data[-i]=Table<Item>.Parse(input,(p,c)=>new Item {HasBug=false});
            }
            for(int i=-199;i<=199;i++)
            {
                var d=Data[i-1];
                Data[i].OutsideValue=(p)=>GetOutsideValue(p,d);
            }
        }

        public Item GetOutsideValue((int x,int y) pos,Table<Item> outsideData)
        {
            var (x,y)=pos;
            if (x<0) return outsideData[1,2];
            if (x>=5) return outsideData[3,2];
            if (y<0) return outsideData[2,1];
            if (y>=5) return outsideData[2,3];
            return null;

        }
        public void Iterate()
        {
            foreach (var (p,item) in Data[0].Each((p,i)=>true))
                item.AdjacentCount=directions.Sum(d=>(Data[0][(p.X+d.dx,p.Y+d.dy)]?.HasBug??false)?1:0);

            foreach (var (p,item) in Data[0].Each((p,i)=>true))
                item.HasBug=(item.HasBug&&item.AdjacentCount==1)||(!item.HasBug&&(item.AdjacentCount==1||item.AdjacentCount==2));
          
        }

        public int GetAdjacentCount(int depth, (int x,int y) currentCalculated, (int x,int y) position)
        {
            if (currentCalculated==(2,2)) return 0;

            if (position==(2,2) && depth<200)
            {
                var d=Data[depth+1];
                if (currentCalculated.x==1) return Enumerable.Range(0,5).Count(x=>d[0,x].HasBug);
                if (currentCalculated.x==3) return Enumerable.Range(0,5).Count(x=>d[4,x].HasBug);
                if (currentCalculated.y==1) return Enumerable.Range(0,5).Count(x=>d[x,0].HasBug);
                if (currentCalculated.y==3) return Enumerable.Range(0,5).Count(x=>d[x,4].HasBug);
                return 0;
            }
            else
            {
                return (Data[depth][position]?.HasBug??false)?1:0;
            }
        }

        public void Iterate2()
        {
            foreach (var da in Data)
                foreach (var (p,item) in da.Value.Each((p,i)=>true)) {

                    item.AdjacentCount=directions.Sum(d=>GetAdjacentCount(da.Key, p, (p.X+d.dx,p.Y+d.dy)));
                }

            foreach (var da in Data)
                foreach (var (p,item) in da.Value.Each((p,i)=>true))
                    item.HasBug=(item.HasBug&&item.AdjacentCount==1)||(!item.HasBug&&(item.AdjacentCount==1||item.AdjacentCount==2));
          
        }

        public void Dump()
        {
            for (int i=-200;i<200;i++)
            {
                var d=Data[i];
                var c=d.Each((p,i)=>i.HasBug).Count();
                if (c>0) {
                    Console.WriteLine($"Level {i} :");
                    Console.WriteLine(d.Dump((p,i)=>i.HasBug?"#":p==(2,2)?"?":"."));
                }
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
           var input =@"#.#..
.#.#.
#...#
.#..#
##.#.";
            var lifeGame=new LifeGame(input);
            var dico=new Dictionary<int,int>();

            while(true)
            {
                lifeGame.Iterate();
                var rating=lifeGame.BioDiversityRating;
                if (dico.TryGetValue(rating,out var counter))
                {
                    Console.WriteLine($"Result = {rating}");
                    break;
                } else
                    dico[rating]=1;
            }

            var lifeGame1=new LifeGame(input);

            for (int i=0;i<200;i++)
            {   
                lifeGame1.Iterate2();
            }
            lifeGame1.Dump();
            Console.WriteLine($"Bug Count={lifeGame1.BugCount}");
        }
    }
}
