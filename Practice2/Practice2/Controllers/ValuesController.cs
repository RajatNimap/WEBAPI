//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Practice2.Controllers
//{
//    [ApiController]

//    [Route("[controller]/[action]")]

//    public class ValuesController : ControllerBase
//    {
//        //[Route("/api/get")]
//        //[Route("getalldata")]
//        //[Route("getdata")]

//        [Route("~/data")]
//        public string getAll() {

//            return "Hello World";
//        }
//        [Route("{id}")]
//        public string get(int id)
//        {
//            return "Happy World " +id ;
//        }


//        public string actionm(int id)
//        {
//            return "this is token replacement";
//        }

//        //[Route("books")]

//        public string getbook()
//        {
//            return "Here are the some books";
//        }
//        [Route("{id}/author/{name}")]
//        public string getbookbyidandName(int id ,string name)
//        {
//            return "Here are the some book id " +id+ " and author name "+name;
//        }

//        [Route("{id}")]
//        public string getbookbyId(int id)
//        {
//            return "Here are the some book id " + id ;
//        }
//        [Route("search")]
//        public string SearchBooks(int? id,string? name,int? rating,int? price)
//        {
//            return $" here are the some query data id = {id} name = {name} rating = {rating} price = {price} ";
//        }

        

//    }
//}
