using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MvcLearning.IServices;
using MvcLearning.Model;
using MvcLearning.Model.Entities;
using MvcLearning.Models;

namespace MvcLearning.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITodoListCrud _todoListCrud;
        public HomeController(ILogger<HomeController> logger, ITodoListCrud todolist)
        {
            _logger = logger;
            _todoListCrud = todolist;
        }
        [HttpGet("~/")]
        public async Task<IActionResult> Index()
        {
            var data = await _todoListCrud.GettodoListData();
            return View(data);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ListDes(int id)
        {
            var data = await _todoListCrud.GetListbyId(id);
            return View(data);
        }
        [HttpGet("create")]
        public async Task<IActionResult> AddTodolIst()
        {
            return View();
        }

        [HttpPost("post")]
        public async Task<IActionResult> create(TodoModelDto todoModel)
        {

            await _todoListCrud.GetTodoListInsert(todoModel);
            return RedirectToAction("Index");
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _todoListCrud.GetListbyId(id);

            //await _todoListCrud.GetTodoUpdate(id, todoModel);
            //return RedirectToAction("ListDes")
            return View(data);

        }

        [HttpPost("UpdateTodo")]
        public async Task<IActionResult> UpdateTodo(TodoModel todoModel)
        {

            await _todoListCrud.GetTodoUpdate(todoModel.Id, todoModel);

            return RedirectToAction("ListDes", new { id = todoModel.Id });
        }
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _todoListCrud.GetTodoListDelete(id);
            return RedirectToAction("Index");   

        }
        
      

        [HttpGet("serach")]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _todoListCrud.Serach(query);
            return View("Index",result);    
        }
        public IActionResult Privacy()
        {
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public int Detail(int id)
        {
            return id;
        }
    }
}
