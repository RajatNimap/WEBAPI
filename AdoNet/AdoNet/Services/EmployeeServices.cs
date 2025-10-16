using AdoNet.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using System.Data;

namespace AdoNet.Services
{
    public class EmployeeServices : IEmployee
    {
        private readonly DataContext _database;
        private readonly IConfiguration _config;
        public EmployeeServices(DataContext database, IConfiguration config )
        {
            _database = database;
            _config = config;
        }
        public async Task<Employee> Addemployee(EmployeeModel employee)
        {


            

                   var data = new Employee()
                   {
                       Name = employee.Name,
                       Position = employee.Position,
                       Age = employee.Age,
                       Departement = employee.Departement
                   };
            _database.employees.Add(data); 
            await _database.SaveChangesAsync();
            return data;    

        }

        public Task<bool> Deleteemployee(int id)
        {
            var data = _database.employees.FirstOrDefault(x => x.Id == id);
            if (data != null)
            {
                _database.employees.Remove(data);
                _database.SaveChangesAsync();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

     

        public async Task<Employee> Getemployeebyid(int id)
        {

            
            var data = await  _database.employees.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<List<Employee>> Getemployees()
        {

            var cs = _config.GetConnectionString("DefaultConnection");
            using(SqlConnection con= new SqlConnection(cs))
            {
                try
                {
                    con.Open();
                    //if (con.State == ConnectionState.Open)
                    //{
                    //    Console.WriteLine("Connection is open");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Connection is closed");
                    //}
                    string query = "select * from employees";
                    string sp = "test";
                    //if you don't provide query and conn you have another approch 
                    SqlCommand cmd = new SqlCommand(sp,con);
                    //SqlCommand cmd = new SqlCommand();
                    //cmd.CommandText = "Select * from employees";
                    //cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Console.WriteLine($"{dr["Id"]} {dr["Name"]} {dr["Position"]} {dr["Age"]}");
                    }
                
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            var data =  await _database.employees.ToListAsync();
            return data;
        }

        public async Task<Employee> Updateemployee(int id, EmployeeModel employee)
        {

                        var data = _database.employees.FirstOrDefault(x => x.Id == id);     
                        data.Name = employee.Name;
                        data.Position = employee.Position;              
                        data.Age = employee.Age;    
                        data.Departement = employee.Departement;    
                        await _database.SaveChangesAsync();
                        return data;

        }
    }
}
