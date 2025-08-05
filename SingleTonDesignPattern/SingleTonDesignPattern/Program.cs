

using SingleTonDesignPattern;

public class Program
{


    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World");
        Singleton singleton1 = Singleton.Getinstance();
        Singleton singleton2 = Singleton.Getinstance();
        Singleton singleton3 = Singleton.Getinstance();

        //from the derived class    
        Singleton.SecondClass singleton4 = new Singleton.SecondClass();
        singleton4.PrintDetail("Derived Class Instance");

        singleton1.PrintDetail("First Singleton Instance");
        singleton2.PrintDetail("Second Singleton Instance");
        singleton3.PrintDetail("Third Singleton Instance"); 
    }

}