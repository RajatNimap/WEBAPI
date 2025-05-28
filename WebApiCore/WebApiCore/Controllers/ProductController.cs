using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApiCore.Model;

namespace WebApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnetion()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        }  

        [HttpGet]
        public IActionResult Getdata()
        {
            var Data = new List<Product>();
            DataTable dt = new DataTable();
            var conn = GetConnetion();
            SqlCommand sqlCommand = new SqlCommand("Select * from ListOfProduct", conn);
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            adapter.Fill(dt);

            foreach (DataRow row in dt.Rows) {

                Product product = new Product()
                {
                    ProductId = Convert.ToInt32(row["ID"]),
                    ProductName = row["Pname"].ToString(),
                    ProductPrice = Convert.ToDecimal(row["Price"]),
                    dateTime = Convert.ToDateTime(row["DateOfOrder"])

                };

                Data.Add(product);

            }

            return Ok(Data);

        }
        [HttpPost]
        public IActionResult Postdata(ProductDto product) {
         
            try
            {
                using (var conn= GetConnetion())
                {
                    conn.Open();

                    string query = "INSERT INTO ListOfProduct (Pname, Price, DateOfOrder) VALUES (@ProductName, @ProductPrice, GETDATE())";

                    using (SqlCommand sqlCommand = new SqlCommand(query, conn))
                    {
                        sqlCommand.Parameters.AddWithValue("@ProductName", product.ProductName);
                        sqlCommand.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);

                        int rowsAffected = sqlCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return Ok("Product inserted successfully.");
                        else
                            return StatusCode(500, "Failed to insert product.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
        [HttpPut("{id}")]
        public IActionResult Putdata(int id, ProductDto product) {

            try
            {

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    string queryforupdateion = "Select * from ListOfProduct Where Id=@id";
                    string query = "Update ListOfProduct Set Pname=@ProductName,Price=@ProductPrice,DateOfOrder=GETDATE() where Id=@id";
                    using (SqlCommand sqlCommand = new SqlCommand(query, conn))
                    {
                        sqlCommand.Parameters.AddWithValue("@ProductName", product.ProductName);
                        sqlCommand.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);
                        sqlCommand.Parameters.AddWithValue("@id", id);


                        int rowAffected = sqlCommand.ExecuteNonQuery();
                        if (rowAffected > 0)
                        {

                            return Ok("Product Successfully Updated");
                        }
                        else
                        {
                            return StatusCode(500, "Failed to Insert");
                        }
                    }
                }


            } catch (Exception ex)
            {

                return BadRequest(ex);
            }


        }
        [HttpDelete("Id")]
        public IActionResult Deletedata(int id) {

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {

                    conn.Open();
                    string query = "Delete from ListOfProduct Where Id=@id";

                    using (SqlCommand sqlCommand = new SqlCommand(query,conn))
                    {
                        sqlCommand.Parameters.AddWithValue("@id",id);

                        int rowAffected = sqlCommand.ExecuteNonQuery();
                        if (rowAffected > 0)
                        {

                            return Ok("Product Successfully Deleted");
                        }
                        else
                        {
                            return StatusCode(500, "Failed to Insert");
                        }

                    }
                   

                }


            } catch (Exception ex) {
            
                    return BadRequest(ex);  
            
            
            }


        
        }

    } 

        
        
       
    
}
