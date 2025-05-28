using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApiCore.Model;

namespace WebApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreprocedureController : ControllerBase
    {
        private readonly IConfiguration _configuration; 
        public StoreprocedureController(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        private SqlConnection sqlconn()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }


        [HttpGet]
        public IActionResult Get() {
            try
            {
                using (var conn = sqlconn())
                {
                              conn.Open();

                    var Data = new List<Product>();
                    
                    SqlCommand cmd = new SqlCommand("GetProduct", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };


                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read()) { 
                    
                        Product product =new Product
                        {
                            ProductId = Convert.ToInt32(reader["Id"]),
                            ProductName=reader["Pname"].ToString(),
                            ProductPrice = Convert.ToDecimal(reader["Price"]),
                            dateTime = Convert.ToDateTime(reader["DateOfOrder"])
                        };
                        Data.Add(product);
                    
                    }
                    return Ok(Data);
                }
            }
            catch(Exception ex) 
            {

                return BadRequest(ex);
            }

        
        }
        [HttpGet("GetDataById/{id}")]

        public IActionResult GetDataById(int id) {


            try
            {
                using (var conn = sqlconn())
                {
                    conn.Open();

                    var Data = new List<Product>();

                    SqlCommand cmd = new SqlCommand("GETPRODUCTBYID", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlParameter param1 = new SqlParameter
                    {
                        ParameterName = "@ID",   
                        SqlDbType = SqlDbType.Int,
                        Value = id  ,
                        Direction = ParameterDirection.Input,   
                       
                    };
                    cmd.Parameters.Add(param1);   
                    
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        Product product = new Product
                        {
                            ProductId = Convert.ToInt32(reader["Id"]),
                            ProductName = reader["Pname"].ToString(),
                            ProductPrice = Convert.ToDecimal(reader["Price"]),
                            dateTime = Convert.ToDateTime(reader["DateOfOrder"])
                        };
                        Data.Add(product);

                    }
                    return Ok(Data);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }




        }
        [HttpPost]
        public IActionResult UpdateProduct([FromBody]ProductDto product) { 
                    
            try
            {
                using (var conn = sqlconn())
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand("InsertData", conn)
                    {
                        CommandType = CommandType.StoredProcedure   
                    };

                  
                    SqlParameter param1 = new SqlParameter()
                    {
                            ParameterName="@Name",
                            SqlDbType = SqlDbType.VarChar,  
                            Value = product.ProductName,
                            Direction = ParameterDirection.Input,  
                    };
                    cmd.Parameters.Add(param1);
                    SqlParameter param2 = new SqlParameter
                    {
                        ParameterName = "@Price",
                        SqlDbType = SqlDbType.Decimal,
                        Value = product.ProductPrice,
                        Direction = ParameterDirection.Input,
                    };   
                    cmd.Parameters.Add(param2);
                    SqlParameter parame3 = new SqlParameter
                    {
                        ParameterName = "@Orderdate",
                        SqlDbType = SqlDbType.Date,
                        Value = product.dateTime,
                        Direction = ParameterDirection.Input,
                    };
                    cmd.Parameters.Add(parame3);

                        int executed=cmd.ExecuteNonQuery();
                    if (executed > 0) {
                        return Ok("data added successfully");
                    }
                    else
                    {
                        return BadRequest();
                    }

                }
            }
            catch(Exception ex)
            {
                var error = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source
                };
                return BadRequest(error);  
            }
        
        
        }


        [HttpPut("update/{id}")]
        public IActionResult Post(int id, [FromBody] ProductDto product)
        {

            try
            {
                using (var conn = sqlconn())
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand("updateproduct", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlParameter param0 = new SqlParameter()
                    {
                        ParameterName = "@ID",
                        SqlDbType = SqlDbType.Int,
                        Value = id,
                        Direction = ParameterDirection.Input,
                    };
                    cmd.Parameters.Add(param0);
                    SqlParameter param1 = new SqlParameter()
                    {

                        ParameterName = "@Name",
                        SqlDbType = SqlDbType.VarChar,
                        Value = product.ProductName,
                        Direction = ParameterDirection.Input,
                    };
                    cmd.Parameters.Add(param1);
                    SqlParameter param2 = new SqlParameter
                    {
                        ParameterName = "@Price",
                        SqlDbType = SqlDbType.Decimal,
                        Value = product.ProductPrice,
                        Direction = ParameterDirection.Input,
                    };
                    cmd.Parameters.Add(param2);
                    SqlParameter parame3 = new SqlParameter
                    {
                        ParameterName = "@Orderdate",
                        SqlDbType = SqlDbType.Date,
                        Value = product.dateTime,
                        Direction = ParameterDirection.Input,
                    };
                    cmd.Parameters.Add(parame3);

                    int executed = cmd.ExecuteNonQuery();
                    if (executed > 0)
                    {
                        return Ok("data added successfully");
                    }
                    else
                    {
                        return BadRequest();
                    }

                }
            }
            catch (Exception ex)
            {
                var error = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source
                };
                return BadRequest(error);
            }


        }

        [HttpDelete("Deletedata/{id}")]

        public IActionResult Deletedata(int id)
        {

            try
            {
                using (var conn = sqlconn())
                {
                    conn.Open();

                    var Data = new List<Product>();

                    SqlCommand cmd = new SqlCommand("DeleteData", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlParameter param1 = new SqlParameter
                    {
                        ParameterName = "@ID",
                        SqlDbType = SqlDbType.Int,
                        Value = id,
                        Direction = ParameterDirection.Input,

                    };
                    cmd.Parameters.Add(param1);

                    int executed = cmd.ExecuteNonQuery();
                    if (executed > 0)
                    {
                        return Ok("data Deleted successfully");
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }




        }



    }
}
