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

        [HttpGet]
        public IActionResult Getdata()
        {
            var Data=new List<Product>(); 
            
            DataTable  dt=new DataTable();
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand sqlCommand = new SqlCommand("Select * from ListOfProduct", conn);
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            adapter.Fill(dt);

            //for(int i = 0; i < dt.Rows.Count; i++)
            //{
            //    Product Product = new Product();
            //    Product.ProductId = Convert.ToInt32(dt.Rows[i]["ID"]);
            //    Product.ProductName = dt.Rows[i]["Pname"].ToString();
            //    Product.ProductPrice = Convert.ToDecimal(dt.Rows[i]["Price"]);
            //    Product.dateTime = Convert.ToDateTime(dt.Rows[i]["DateOfOrder"]);

            //    Data.Add(Product);
            //}
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
            //try
            //{

            //    SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            //    SqlCommand sqlCommand = new SqlCommand("Insert into ListOfProduct value ('" + product.ProductName + "," + product.ProductPrice + ",getdate()')", conn);
            //}
            //catch (Exception ex) { 


            //            return BadRequest(ex.Message);  
            //}
            //return Ok(product);
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
    }
}
