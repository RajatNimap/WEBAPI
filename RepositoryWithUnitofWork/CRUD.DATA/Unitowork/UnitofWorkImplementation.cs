using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUD.DATA.Infrastructure;
using CRUD.MODEL.Entities;

namespace CRUD.DATA.Unitowork
{
    public class UnitofWorkImplementation : IUnitofWork
    {
        public  IRepository<Employee> employe { get; }

        private readonly DataContext database;
        public UnitofWorkImplementation(DataContext _database, IRepository<Employee> repository)
        {
            employe=repository;
            database = _database;   
        }
        public async Task SaveCommitChanges()
        {
            await database.SaveChangesAsync();  
        }
        public void Dispose()
        {
            database.Dispose();
        }

    }
}
