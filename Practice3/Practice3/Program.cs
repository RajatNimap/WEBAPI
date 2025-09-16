using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

class Program
{
    static void Main()
    {
        List<Student> students = new List<Student>
        {
            new Student { Id = 1, Name = "John", Age = 25, Grade = "A" },
            new Student { Id = 2, Name = "Jane", Age = 22, Grade = "B" },
            new Student { Id = 3, Name = "Mike", Age = 19, Grade = "A" },
            new Student { Id = 4, Name = "Sarah", Age = 21, Grade = "A" }
        };

        Console.WriteLine("=== Execution Process ===");

        // Step 1: Create IQueryable
        //IQueryable<Student> query = students.AsQueryable();
        //Console.WriteLine($"Query type: {query.GetType()}");
        //Console.WriteLine($"Provider: {query.Provider.GetType()}");

        // Step 2: Add Where clause
        IQueryable<Student> query = students.Where(s => s.Age > 20 && s.Grade == "A");
        Console.WriteLine($"Expression: {query.Expression}");

        // Step 3: Execute the query
        Console.WriteLine("\nExecuting query...");
        var results = query.ToList();

        Console.WriteLine($"Results found: {results.Count}");
        foreach (var student in results)
        {
            Console.WriteLine($"- {student.Name} (Age: {student.Age}, Grade: {student.Grade})");
        }
    }
}

class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Grade { get; set; }
}