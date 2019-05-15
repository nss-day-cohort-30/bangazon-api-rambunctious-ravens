﻿//Author: Niall Fraser
//Purpose: This controller handles all methods for payment types

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
    public class PaymentTypeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PaymentTypeController(IConfiguration config)
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

        // GET
        // GET Purpose: Get method for all payment methods
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //SQL command to execute
                    cmd.CommandText = $"SELECT Id, AcctNumber, [Name], CustomerId FROM PaymentType";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<PaymentType> paymentTypes = new List<PaymentType>();
                    while (reader.Read())
                    {
                        //Create new payment type using the data from the database
                        PaymentType paymentTye = new PaymentType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        };
                        //Add payment type to collection
                        paymentTypes.Add(paymentTye);
                    }

                    reader.Close();
                    // return payment types with 200 status code 
                    return Ok(paymentTypes);
                }
            }
        }

        // GET api/values/5
        // Purpose: Get individual payment type by Id.
        [HttpGet("{id}", Name = "GetPaymentType")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (!PaymentTypeExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, AcctNumber, Name, CustomerId FROM PaymentType
                                         WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    PaymentType paymentType = null;

                    if (reader.Read())
                    {
                            paymentType = new PaymentType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                            };
                    }
                    
                    reader.Close();

                    return Ok(paymentType);
                }
            }
        }

        //POST api/values
        //Purpose: Add a new payment type to the database
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentType pt)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                                            INSERT INTO PaymentType (Acctnumber, Name, CustomerId)
                                            OUTPUT INSERTED.Id
                                            VALUES (@acctNumber, @name, @customerId)
                        ";
                    cmd.Parameters.Add(new SqlParameter("@acctNumber", pt.AcctNumber));
                    cmd.Parameters.Add(new SqlParameter("@name", pt.Name));
                    cmd.Parameters.Add(new SqlParameter("@customerId", pt.CustomerId));
                    pt.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetPaymentType", new { id = pt.Id }, pt);
                }
            }
        }

        // PUT api/values/5
        // Purpose: Edit a specific payment type in the database by passing in the Id that you want to edit
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PaymentType pt)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE PaymentType
                            SET Acctnumber = @acctNumber,
                                Name =  @name,
                                CustomerId = @custId
                                WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@acctNumber", pt.AcctNumber));
                        cmd.Parameters.Add(new SqlParameter("@name", pt.Name));
                        cmd.Parameters.Add(new SqlParameter("@custId", pt.CustomerId));
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
            catch (Exception)
            {
                if (!PaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //DELETE api/values/5
        //Purpose: delete a payment type by passing in the Id of the payment type you want to delete
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
                        cmd.CommandText = @"DELETE FROM PaymentType WHERE Id = @id";
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
            catch (Exception)
            {
                if (!PaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //Bool to check that a payment type exists in the database
        private bool PaymentTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //More string interpolation
                    cmd.CommandText = "SELECT Id FROM PaymentType WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}

