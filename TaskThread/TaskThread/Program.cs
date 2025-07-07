using System;


using System.Threading;

namespace TaskThread
{
    public class Program
    {


        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World");

            Display();
            Console.WriteLine("PenPencil");
            Console.ReadKey(); 

        }

        public  static void Display()
        {
            Console.WriteLine("display before");
              Thread.Sleep(2000);
            Console.WriteLine("display after ");
                
        }
    }
}