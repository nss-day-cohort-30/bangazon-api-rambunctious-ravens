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
    // This class holds all methods for basic CRUD for the product types.Author: Jacob Sanders
    public class ProductTypeController : Controller
    {
        private readonly IConfiguration _config;

        public ProductTypeController(IConfiguration config)
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
        [HttpGet]
        //This method returns all instances of product types from the database
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // This is the SQL statement that is given to the database to get all of the product types 
                    cmd.CommandText = "SELECT Id, [Name] FROM ProductType";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    // Creates a list of product types to loop through and display
                    List<ProductType> productTypes = new List<ProductType>();
                    //Loops through the product type list 
                    while (reader.Read())
                    {
                        //Creates the objects individually 
                        ProductType productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                        // Adds the instance of each product type to the product ype list
                        productTypes.Add(productType);
                    }

                    reader.Close();

                    return Ok(productTypes);
                }
            }
        }
        //This method gets an individual product type by the id that is submitted
        [HttpGet("{Id}", Name = "GetProductType")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //This is the SQL statement that is sent to the database to return the specified product type
                    cmd.CommandText = "SELECT Id, [Name] FROM ProductType WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    ProductType productType = null;
                    if (reader.Read())
                    {
                        // Takes the data that was returned from the database and creates an object with it 
                        productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };
                    }
                    // Ensures the specified product type exists in the database 
                    if (!ProductTypeExists(id))
                    { return NotFound(); };
                    reader.Close();

                    return Ok(productType);
                }
            }
        }
        //This method adds a new product type to the database, it uses the product type as an argument to ensure the correct formating 
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductType productType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // This is the SQL statement that sends the data to the database to be saved 
                    cmd.CommandText = @"
                            INSERT INTO ProductType ([Name])
                            OUTPUT INSERTED.Id
                            VALUES (@name)
                        ";
                    cmd.Parameters.Add(new SqlParameter("@name", productType.Name));
                    productType.Id = (int)await cmd.ExecuteScalarAsync();
                    // Searches the database for where the Id's end and creates a new instance using the given info with a new Id
                    return CreatedAtRoute("GetProductType", new { id = productType.Id }, productType);
                }
            }
        }
        //This method updates an existing instance of a product type within the database with new information
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductType productType)
        {
            //This trys the code and if it fails catches it and throws an exception
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        // This is the SQL statement that is passed through to find the product type you wish to update and updates the new values 
                        cmd.CommandText = @"
                                UPDATE ProductType
                                SET Name = @name
                                WHERE Id = @id
                            ";
                        // This adds new SQL parameters and assigns them to the correct product values 
                        cmd.Parameters.Add(new SqlParameter("@name", productType.Name ));
                        cmd.Parameters.Add(new SqlParameter("@id", (id)));
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        // Checks the database to see if the update went through by seeing if any of the rows were affected 
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        // This is the exception that is thrown if the code fails
                        throw new Exception("No rows affected");
                    }
                }
            }
            // This catches the failure of the code and returns 
            catch (Exception)
            {
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        //This method deletes an instance of product type from the database by using the given id in the argument
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //This trys out the code and if it fails catches it
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        //This is the SQL statement that is passed to the database in order to delete a specific instance of product type
                        cmd.CommandText = @"DELETE FROM ProductType WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        //Ensures the instance was deleted by checking if any rows were affected once the delete method went through
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        // the exception that is thrown if the code fails
                        throw new Exception("No rows affected");
                    }
                }
            }
            // This checks if the product type exists and if it does returns not found else tells you nothing was changed
            catch (Exception)
            {
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        // This checks if the product type exists by getting the product type by id
        private bool ProductTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // This is the SQL statement that is sent to the database so that the product type instance can be found 
                    cmd.CommandText = "SELECT Id FROM ProductType WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }

        }
    }
}



    
