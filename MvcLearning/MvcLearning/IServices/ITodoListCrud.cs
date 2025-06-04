using MvcLearning.Model;
using MvcLearning.Model.Entities;

namespace MvcLearning.IServices
{
    public interface ITodoListCrud
    {
        Task<List<TodoModel>> GettodoListData();
        Task<TodoModel> GetListbyId(int id);
        Task GetTodoListInsert(TodoModelDto todoModelDto);
        Task GetTodoUpdate(int id,TodoModel todoModelDto);
        Task GetTodoListDelete(int id);
        Task <List<TodoModel>> Serach(string str);
    }
}
