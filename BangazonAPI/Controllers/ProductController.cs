using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // This class holds all methods for basic CRUD for the products. Author: Jacob Sanders
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET api/values
        // This method searches the database for all products and returns them.
        // For it to work you must have previously ran the SQL statements for products, product types, and customers which can be found in the readme 
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //This creates and SQL command which gives you information on the product, the product Type, and the customer which will be assigned at later point 
                    cmd.CommandText = @"SELECT p.Id productId, pt.Name, p.ProductTypeId, p.CustomerId, pt.Id pTId, c.Id cId, c.FirstName , c.LastName, p.Price, p.Title, p.Description, p.Quantity 
                                        FROM Product p
                                        JOIN ProductType pt 
                                        ON p.ProductTypeId = pt.Id 
                                        JOIN Customer c
                                        ON p.CustomerId = c.Id";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //Creates a list of Products to loop through
                    List<Product> products = new List<Product>();
                    while (reader.Read())
                    {
                        //This creates a single object for each Product and assigns the values in the correct places
                        Product product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("productId")),
                            Price = reader.GetInt32(reader.GetOrdinal("Price")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            ProductType = new ProductType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("pTId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            },
                            Customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("customerId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))

                            },
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            
                        };
                        // Adds the product instance to the product list
                        products.Add(product);
                    }

                    reader.Close();

                    return Ok(products);
                }
            }
        }

        // GET api/values/5
        //This method returns a single product by id which is specified in the url
        // For it to work you must have previously ran the SQL statements for products, product types, and customers which can be found in the readme 
        [HttpGet("{Id}", Name ="GetProduct")]
        //The id for the url is taken in through the methods parameter
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                        cmd.CommandText = @"SELECT p.Id productId, pt.Name, p.ProductTypeId, p.CustomerId, pt.Id pTId, c.Id cId, c.FirstName , c.LastName, p.Price, p.Title, p.Description, p.Quantity 
                                        FROM Product p
                                        JOIN ProductType pt 
                                        ON p.ProductTypeId = pt.Id 
                                        JOIN Customer c
                                        ON p.CustomerId = c.Id
                                        WHERE p.Id = @id";
                         //Takes the id from the parameter and assigns it to the SQL statement 
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Product product = null;
                        if (reader.Read())
                        { 
                        //Takes the info from the data that SQL returned and creates an instance of that single object.
                            product = new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("productId")),
                                Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                ProductType = new ProductType
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("pTId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                },
                                Customer = new Customer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("customerId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))

                                },
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            };
                        }
                        //If no such product exists this returns a 404 error
                    if (!ProductExists(id))
                    { return NotFound(); };

                    reader.Close();

                    return Ok(product);
                }
            }
        }

        // POST api/values
        //This method adds a new instance of a product to the database
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Creates the new object from the information that is provided
                    cmd.CommandText = @"
                            INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, Description, Quantity)
                            OUTPUT INSERTED.Id
                            VALUES (@productTypeId, @customerId, @price, @title, @description, @quantity)
                        ";
                    //Provides the information to the SQL statement 
                    cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@customerId", product.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                    cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                    cmd.Parameters.Add(new SqlParameter("@quantity", product.Quantity));
                    product.Id = (int)await cmd.ExecuteScalarAsync();
                    // Searches the database for an empty id and assigns the new instance to it
                    return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        //This method modifys an instance of a product within the database, The id is used to target the correct instance to update
        // The Product is passed through to ensure that we can have the correct formating 
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        //Sends the SQL to the database to update the selected object
                        cmd.CommandText = @"
                                UPDATE Product
                                SET ProductTypeId = @productTypeId,
                                    CustomerId = @customerId,
                                    Price = @price,
                                    Title = @title,
                                    Description = @description,
                                    Quantity = @quantity
                                WHERE Id = @id
                            ";
                        // Assigns the new values for the SQL to update the database with
                        cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                        cmd.Parameters.Add(new SqlParameter("@customerId", product.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                        cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                        cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                        cmd.Parameters.Add(new SqlParameter("@quantity", product.Quantity));
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        // Ensures the data was modified by checking if any rows were affected
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        // Throws an exception if no rows were affected
                        throw new Exception("No rows affected");
                    }
                }
            }
            // Ensures the product that you are wanting to update exists
            catch (Exception)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/values/5
        //This method deletes an instance of a product in the database which is specified by the id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        //This is the SQL that is sent to the database to delete the specified information 
                        cmd.CommandText = @"DELETE FROM Product WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        //Ensures the instance was deleted by checking how many rows were affected
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        //This takes in the id of an instance within the database and checks to ensure that the instance at the spot specified by the id exists
        private bool ProductExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // This is the SQL statement that is sent to the database to check if the instance correlated to the given id exists
                    cmd.CommandText = "SELECT Id FROM Product WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}


