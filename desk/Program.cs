using desk.domain;
using System;
using System.Text.RegularExpressions;

namespace AllMediaDesk
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Enter term below to get calculated");

            var equation = Console.ReadLine();

            Bodmas math = new Bodmas();
            var result = math.Calculate(equation);

            Console.WriteLine($"Result = {result}");
            Console.ReadLine();
        }
    }
}
