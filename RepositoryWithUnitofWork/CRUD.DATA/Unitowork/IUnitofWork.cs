using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUD.DATA.Infrastructure;
using CRUD.MODEL.Entities;

namespace CRUD.DATA.Unitowork
{
    public interface IUnitofWork :IDisposable
    {
       IRepository<Employee> employe { get; }    
       Task SaveCommitChanges();    

    }
}
