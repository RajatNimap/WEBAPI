
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using LInq;

public class Program
{

    public static void ListQuery(){

        string[] names = new[] { "Rajat", "pandit", "Suraj", "akshay" };

        var res = from str in names
                  where str.StartsWith("R")
                  select str;

        foreach (var i in res)
        {
            Console.WriteLine(i);
        }
    }
    public static void DbSelectQuery()
    {
        using var context = new DataContext();
        var User = from customer in context.users
                   where customer.Id > 10 
                   orderby customer.Name ascending
                   select new
                   {
                       Id=customer.Id,
                       Name=customer.Name,  
                       Age=customer.age
                       
                   };

        var User1 = context.users.Where(x => x.Id > 10).OrderBy(x => x.age).Select(x => new {x.Name,x.age}).ToList();
        foreach (var i in User1)
        {
            //Console.WriteLine($"{i.Id} {i.Name} {i.Email} {i.age} {i.Password}");
            Console.WriteLine($"{i}");

        }
    }

    //Joining 
    public static void Joining()
    {
        using var context = new DataContext();
        var jointabale = from product in context.products
                         join Category in context.categories
                         on product.CategoryID equals Category.Id
                         orderby product.Name ascending
                         select new
                         {
                             productName = product.Name,
                             CategoryName = Category.Name,
                         };
       
        jointabale = jointabale.Skip(1).Take(5);                 
                         foreach(var i in jointabale)
                         {
                             Console.WriteLine(i);
                         }
    }

    public static void Groupby()
    {
        using var context =new DataContext();
        var jointable = from product in context.products
                        group product by product.CategoryID into g
                        select new {
                            CategoryId = g.Key,
                            ProductName = g.ToList()
                        
                        };

        foreach (var group in jointable)
        {
            Console.WriteLine($"CategoryID: {group.CategoryId}");

            foreach (var product in group.ProductName)
            {
                Console.WriteLine($" - {product.Name}");
            }
        }

    }
    public static void Main(string[] args)
    {
        //ListQuery();
        //DbSelectQuery();
        //Joining();
        Groupby();

       //var voice=new Sound();
       //voice.makesound();

    }
    public class Sound : Iabstract
    {
        public void makesound()
        {
            Console.WriteLine("Make sound");
        }
    }

}