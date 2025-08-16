using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalQuestion
{
    static class PatternProblem
    {
        public static void PrintPattern(int n)
        {
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n - i; j++)
                {
                    Console.Write(" ");
                }
                for (int k = 1; k <= (2 * i - 1); k++)
                {
                    Console.Write("*");
                }
                Console.WriteLine();
            }
        }

        public static void HalfDia(int n)
        {

            for (int i = 1; i <= n; i++)
            {

                for (int j = n; j >= i; j--)
                {
                    Console.Write(" ");
                }
                for (int k = 1; k <=(2*i-1); k++)
                {
                    Console.Write("*");
                }

                Console.WriteLine();
            }

        }
        public static void RevDia(int n) {
        

            for(int i=1; i<=n; i++)
            {
                for (int j=1; j<=i; j++)
                {
                    Console.Write(" ");
                }

                for(int k=n*2-1;k>=i* 2 - 1; k--)
                {
                    Console.Write("*");
                }
                Console.WriteLine();
            }
        }
        public static void FullDiamond(int n)
        {

            for(int i=1; i <= n; i++)
            {

                for (int l = i; l <=n; l++)
                {
                    Console.Write(" ");
                }

                for (int k=1; k <= (2 * i - 1); k++)
                {
                    Console.Write("*");
                }

                Console.WriteLine();

            }

            for (int i=1;i<=n; i++)
            {
                for (int j = 1; j <= i; j++)
                {
                    Console.Write(" ");
                }

                for (int c = n * 2 - 1; c >= i * 2 - 1; c--)
                {
                    Console.Write("*");
                }

                Console.WriteLine();

            }

        }

        public static void straigthalf(int n)
        {


            for (int i = 1; i <= n; i++)
            {

                for (int j = 1; j <= i; j++)
                {

                    Console.Write("*");
                }

                Console.WriteLine();
            }

            for (int q = 1; q <= n - 1; q++)
            {

                for (int k = n - 1; k >= q; k--)
                {
                    Console.Write("*");
                }
                Console.WriteLine();

            }

        }
        public static void Printstar01(int n)
        {

            for (int i = 1; i <= n; i++)
            {
                var bolean = true;

                for (int j = 1; j <= i; j++)
                {
                    int value = bolean ? 1 : 0;
                    Console.Write(value);
                    if (bolean == true)
                    {
                        bolean = false;
                    }
                    else
                    {
                        bolean = true;
                    }
                }
                Console.WriteLine();
            }
        }

        public static void PrintPatterhRJ(int n)
        {
            // 1 2 3 4 5 * * * * * 11 12 13 14 15 * * * * * 21 22 23
            int a = 0;

            for (int i = 1; i <= n; i++)
            {
             
                if (a % 2 == 0)
                {
                    Console.Write($" {i}");
                }
                else
                {
                    Console.Write(" *");
                }

                if (i % 5 == 0)
                {
                    a++;
                }   
            }
        }
    }
}
