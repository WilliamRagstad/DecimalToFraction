using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyDecimalToFraction
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch t = new Stopwatch();
            while(true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                t.Start();
                bool hasValue = Fraction.TryParse(input, out Fraction frac, true);
                t.Stop();
                if (hasValue) {
                    Console.WriteLine($"= {frac}\n{t.ElapsedMilliseconds}ms");
                }
                else Console.WriteLine("Please enter a valid number!");
                t.Reset();
            }
        }
    }
}
