using Microsoft.EntityFrameworkCore;
using MvcLearning.Data;
using MvcLearning.Model;
using MvcLearning.Model.Entities;

namespace MvcLearning.IServices
{
    public class ServiceImplementation : ITodoListCrud
    {
        private readonly DataContext Database;

        public ServiceImplementation(DataContext data)
        {
            Database = data;    
        }
        public async Task<TodoModel> GetListbyId(int id)
        {
             var data = await Database.todoModels.FirstOrDefaultAsync(x=>x.Id == id);
            if (data == null) {
                return null;
            }

            return data;    
        }

        public async Task<List<TodoModel>> GettodoListData()
        {
            var data = await Database.todoModels.ToListAsync();
            if (data == null) {
                return null;
            }
           return data; 
        }

        public async Task GetTodoListDelete(int id)
        {
            var data = await Database.todoModels.FirstOrDefaultAsync(x => x.Id == id);
            if(data != null)
            {
                 Database.todoModels.Remove(data);
                await Database.SaveChangesAsync();  
            }
        }

        public async Task GetTodoListInsert(TodoModelDto todoModelDto)
        {
               var data=new TodoModel
               {
                   Title = todoModelDto.Title,  
                   Description = todoModelDto.Description,  
               };
            await Database.todoModels.AddAsync(data);   
            await Database.SaveChangesAsync();
        }

        public async Task GetTodoUpdate(int id,TodoModelDto todoModelDto)
        {
            var data = await Database.todoModels.FirstOrDefaultAsync(x => x.Id == id);
           
             if(data != null)
             {
                data.Title = todoModelDto.Title;
                data.Description = todoModelDto.Description;
                await Database.todoModels.AddAsync(data);
                await Database.SaveChangesAsync();

             } 
        }
    }
}
