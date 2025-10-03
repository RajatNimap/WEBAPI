


using System.Threading.Tasks;

namespace TaskandThread
{
   
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Console.WriteLine("Hello, World!");

            //Thread th = new Thread(Method1);
            //th.Start();
            //Thread th2 = new Thread(Method2);
            //th2.Start();


            //for (int i = 0; i < 1000; i++)
            //{
            //    Console.WriteLine("Main thread " + i);
            //}
            //await Print1to100();
            //await PrintAtoZ();
            //Task t1 = Print1to100();
            //Task t2 = PrintAtoZ();
            //Task.WaitAll(t1, t2);   
            //var t1 = Task.Run(() => Print1to100());
            //var t2 = Task.Run(() => PrintAtoZ());
            //await Task.WhenAll(t1, t2);

            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<string> name=new List<string>(){ "rajat","pandit","Niraj","Suraj","Akashay"};
            Parallel.For(0, numbers.Length, i =>
            {
                Console.WriteLine(numbers[i]);  
            });
            Parallel.ForEach(name, n =>
            {
                Console.WriteLine(n);
            }); 

        }


        //public static void Method1()
        //{
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        Console.WriteLine("by thread " + i);
        //    }
        //}
        //public static  async void Method2()
        //{
        //    for (int i = 0; i < 1000; i++)
        //    {
        //       Console.WriteLine("by thread " + i);
        //    }
        //}

        //public static async Task Print1to100()
        //{
        //    for (int i = 1; i <= 100; i++)
        //    {
        //        Console.WriteLine(i);
        //        await Task.Delay(100);
        //    }   
        //}
        //public static async Task PrintAtoZ()
        //{
        //    for (char c = 'A'; c <= 'Z'; c++)
        //    {
        //        Console.WriteLine(c);
        //        await Task.Delay(100);
        //    }
        //}
    }
}