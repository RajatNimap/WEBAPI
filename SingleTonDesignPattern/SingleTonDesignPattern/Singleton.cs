using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleTonDesignPattern
{
   sealed class Singleton
    {
        private Singleton()
        {
            Cnt++;
            Console.WriteLine("Singleton Instance Created. Count: " + Cnt);
        }
        private static int Cnt=0;
        private static Singleton instance = null;   
        
       public static Singleton Getinstance()
       {
            if (instance == null)
            {
                instance = new Singleton();
            }
            return instance;    
       }

        public void PrintDetail(string message)
        {
            Console.WriteLine("Message: " + message);
        }
        
        //public class SecondClass : Singleton
        //{
            
        //}

    }
}
