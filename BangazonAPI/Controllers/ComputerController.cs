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
    public class ComputerController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
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
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer FROM Computer";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();


                    List<Computer> computers = new List<Computer>();
                    while (reader.Read())
                    {
                        DateTime? decomissionDate = null;

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            decomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));

                            Computer computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                DecomissionDate = decomissionDate

                            };

                            computers.Add(computer);
                        }

                        else
                        {
                            Computer computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),

                            };

                            computers.Add(computer);
                        }



                    }
                    reader.Close();
                    return Ok(computers);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetComputer")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer FROM Computer c WHERE c.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Computer computer = null;

                    while (reader.Read())
                    {
                        DateTime? decomissionDate = null;

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            decomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));

                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                DecomissionDate = decomissionDate

                            };

                        }

                        else
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),

                            };

                        }
                        // check to see if id exists, if it does not
                        // return status code 404
                    }
                    if (!ComputerExists(id))
                    {
                        return NotFound();
                    }
                    reader.Close();
                    return Ok(computer);
                }
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Computer computer)
        {
            // sql statement to be acted upon if decomission date is null
            string sqlWithOutdecom = @"
                        INSERT INTO Computer (PurchaseDate, Make, Manufacturer)
                        OUTPUT INSERTED.Id
                        VALUES (@PurchaseDate, @Make, @Manufacturer)
                        ";

            // sql statement if decomission date is NOT null
            string sqlWithdecom = @"
                        INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer)
                        OUTPUT INSERTED.Id
                        VALUES (@PurchaseDate, @DecomissionDate, @Make, @Manufacturer)
                        ";

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                if (computer.DecomissionDate == null)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {

                        cmd.CommandText = sqlWithOutdecom;
                        cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@Manufacturer", computer.Manufacturer));

                        computer.Id = (int)await cmd.ExecuteScalarAsync();

                        return CreatedAtRoute("GetComputer", new { id = computer.Id }, computer);
                    }
                }
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = sqlWithdecom;
                    cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));
                    cmd.Parameters.Add(new SqlParameter("@DecomissionDate", computer.DecomissionDate));
                    cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                    cmd.Parameters.Add(new SqlParameter("@Manufacturer", computer.Manufacturer));

                    computer.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetComputer", new { id = computer.Id }, computer);
                }

            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Computer computer)
        {
            // sql statement to be acted upon if decomission date is null
            string sqlWithOutdecom = @"
                        UPDATE Computer 
                        SET Make = @Make,
                            Manufacturer = @Manufacturer,
                            PurchaseDate = @PurchaseDate,
                        WHERE Id = @id;
                        ";
            // sql statement if decomission date is NOT null
            string sqlWithdecom = @"
                        UPDATE Computer 
                        SET Make = @Make,
                            Manufacturer = @Manufacturer,
                            PurchaseDate = @PurchaseDate,
                            DecomissionDate = @DecomissionDate
                        WHERE Id = @id;
                        ";
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    if (computer.DecomissionDate == null)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            // if the decomission date is null, do not include it
                            cmd.CommandText = sqlWithOutdecom;
                            cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));
                            cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                            cmd.Parameters.Add(new SqlParameter("@Manufacturer", computer.Manufacturer));
                            cmd.Parameters.Add(new SqlParameter("@id", id));

                            computer.Id = (int)await cmd.ExecuteScalarAsync();

                            return CreatedAtRoute("GetComputer", new { id = computer.Id }, computer);
                        }
                    }
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        // if the decomission date exists AND is not null, include it
                        cmd.CommandText = sqlWithdecom;
                        cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@DecomissionDate", computer.DecomissionDate));
                        cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@Manufacturer", computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }

                    //computer.Id = (int)await cmd.ExecuteScalarAsync();

                    //    return CreatedAtRoute("GetComputer", new { id = computer.Id }, computer);
                }


            }


            catch (Exception)
            {
                if (!ComputerExists(id))
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
                        // first delete the instance of the item from the join table 
                        cmd.CommandText = "DELETE FROM ComputerEmployee WHERE ComputerId = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        // then delete the original instance of the item from its own table
                        cmd.CommandText = "DELETE FROM Computer WHERE Id = @id";
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
                if (!ComputerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }


        private bool ComputerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // boolean to check to see if employee exist
                    // used a couple times prior
                    cmd.CommandText = @"SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer
                                        FROM Computer 
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
