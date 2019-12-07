using System;

namespace Day6
{
    public class Node
    {
        public Node ParentNode {get;set;}
        public string Name {get;set;}
    }
    public class InputParser
    {        
        public InputParser(string input)
        {

        }

        public static Node ParseOne(string input)
        {
            var r=input.Split(')');
            return new Node {Name=r[1], ParentNode=new Node{Name=r[0]}};
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
