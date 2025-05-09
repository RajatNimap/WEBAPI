//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Practice2.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RouteController : ControllerBase
//    {

//        [Route("{id:int:min(5):max(20)}")]
//        public string getdata(int id)
//        {
//            return "My Name is Rajat Pandit and my id is " + id;

//        }
//        [Route("{id:minlength(5)}")]
//        public string getdatastring(string id)
//        {
//            return "My Name is Rajat Pandit and this is coming from string " + id;

//        }

//        [Route("hi/{str:range(3,6)}")]
//        public int getdatastring2(int str)
//        {
//            return str;
//        }


//        [Route("hii/{str:alpha:minlength(3):maxlength(6)}")]

//        public string getdatastring3(string str) {    return str; }   
//    }
//}
