// This class holds all methods for basic CRUD for the orders. 
// Author: Sam Cronin

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
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrderController(IConfiguration config)
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

        // This method gets all orders from the database by selecting and joining the appropriate (Customer + PaymentType) tables.
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                     
                    // In order to see customer information and payment type details, join those tables on each order.
                     
                    cmd.CommandText = @"SELECT o.Id, o.CustomerId, o.PaymentTypeId, c.FirstName, c.LastName, pt.Name, pt.AcctNumber, pt.CustomerId
                                        FROM [Order] o
                                        JOIN Customer c ON c.Id = o.CustomerId
                                        JOIN PaymentType pt ON pt.Id = o.PaymentTypeId";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();


                    List<Order> orders = new List<Order>();
                    while (reader.Read())
                    {
                        // Create new instance of Order object
                        Order order = new Order
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            },
                            PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                            PaymentType = new PaymentType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                            }
                        };

                        orders.Add(order);
                    };

                    reader.Close();
                    return Ok(orders);
                }
            }
        }

        // GET api/values/5

        // Select all information on on specific order, joining Customer and Payment Type again
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Same statement from GET except passing in an id for the specific order
                    cmd.CommandText = @"SELECT o.Id, o.CustomerId, o.PaymentTypeId, c.FirstName, c.LastName, pt.Name, pt.AcctNumber, pt.CustomerId
                                        FROM [Order] o
                                        JOIN Customer c ON c.Id = o.CustomerId
                                        JOIN PaymentType pt ON pt.Id = o.PaymentTypeId
                                        WHERE o.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Order order = null;
                    if (reader.Read())
                    {
                         order = new Order
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            },
                            PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                            PaymentType = new PaymentType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                            }
                        };
                    }
                    /* check to see if id exists, if it does not
                     return status code 404

                    if it does exist, return an OK status code
                     */
                    if (!OrderExists(id))
                    {
                        return NotFound();
                    }
                    reader.Close();
                    return Ok(order);
   
                }

            }
        }
        
    
        // POST api/values

        // Post new Order object to database, constructor inside
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                    // When the object is created, return the new object's Id so that we can store it in it's Id property
                        cmd.CommandText = @"
                        INSERT INTO [Order] (CustomerId, PaymentTypeId)
                        OUTPUT INSERTED.Id
                        VALUES (@CustomerId, @PaymentTypeId)
                        ";
                        cmd.Parameters.Add(new SqlParameter("@CustomerId", order.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@PaymentTypeId", order.PaymentTypeId));

                        order.Id = (int)await cmd.ExecuteScalarAsync();

                    // Store returned Id as new object's Id and create a url 
                        return CreatedAtRoute("GetOrder", new { id = order.Id }, order);
                    }

            }
        }

        // PUT api/values/5

        // Update Order object that currently exists in the database by passing in it's Id and the new object with changes
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order order)
        {
            // "Try" to update and "Catch" if the id passed in the method does not exist
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                        // All keys must be present on the Order to properly update it, pass in Id in method call
                            cmd.CommandText = @"
                                            UPDATE [Order]
                                            SET CustomerId = @CustomerId,
                                                PaymentTypeId = @PaymentTypeId
                                            WHERE Id = @id
                                            ";
                            cmd.Parameters.Add(new SqlParameter("@CustomerId", order.CustomerId));
                            cmd.Parameters.Add(new SqlParameter("@PaymentTypeId", order.PaymentTypeId));
                            cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                            if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                        }
                }
            }

            // Again, "Try" to update Order object, "Catch" if the Id does not exist
            catch (Exception)
            {
                if (!OrderExists(id))
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

        // Remove Object from database based on the Object's Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // "Try" to remove Ojbect if the Id exists
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        // Delete the original instance of the item from its own table
                        cmd.CommandText = "DELETE FROM [Order] WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            // "Catch" a false Id if it does not exist in the database and return a Not Found status code
            catch (Exception)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }

        /* 
         * HELPER METHOD
         * Boolean used in DELTE PUT GETBYID methods where the Id is pertinent to the success of the method. Those methods are
         Get By Id, Put, and Delete
         */
        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Almost the same as the Get method except that this method returns a boolean
                    cmd.CommandText = @"SELECT Id, CustomerId, PaymentTypeId
                                        FROM [Order] 
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}