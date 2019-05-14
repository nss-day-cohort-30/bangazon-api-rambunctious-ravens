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
    public class TrainingProgramController : Controller
    {
          
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
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
                    cmd.CommandText = "SELECT * from TrainingProgram";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    DateTime? StartDate = null;
                    DateTime? EndDate = null;
                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),

                            // You might have more columns
                        };
                        StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                        EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));

                        trainingPrograms.Add(trainingProgram);

                    }

                    reader.Close();

                    return Ok(trainingPrograms);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"Select Id, Name, StartDate, EndDate, MaxAttendees from TrainingProgram
                                          WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    TrainingProgram trainingProgram = null;
                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),

                            // You might have more columns
                        };

                    }


                    reader.Close();

                    return Ok(trainingProgram);
                }
            }
        }



      //POST api/values
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TrainingProgram trainingprogram)
    {
     using (SqlConnection conn = Connection)
    {
         conn.Open();
         using (SqlCommand cmd = conn.CreateCommand())
        {
               // More string interpolation
               cmd.CommandText = $@"
              INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
                  OUTPUT INSERTED.Id
                 VALUES (@Name, @StartDate, @EndDate, @MaxAttendees) ";
              cmd.Parameters.Add(new SqlParameter("@Name", trainingprogram.Name));
                    cmd.Parameters.Add(new SqlParameter("@StartDate", trainingprogram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@EndDate", trainingprogram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingprogram.MaxAttendees));

                    trainingprogram.Id = (int) await cmd.ExecuteScalarAsync();

            return CreatedAtRoute("GetTrainingProgram", new { id = trainingprogram.Id }, trainingprogram);
           }
       }
        }




        /// PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TrainingProgram trainingprogram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE TrainingProgram
                            SET Name = @Name,
                             StartDate = @StartDate,
                             EndDate = @EndDate,
                             MaxAttendees = @MaxAttendees
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", trainingprogram.Id));
                        cmd.Parameters.Add(new SqlParameter("@Name", trainingprogram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingprogram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingprogram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingprogram.MaxAttendees));

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
            if (!TrainingProgramExists(id))
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
                        cmd.CommandText = @"DELETE from TrainingProgram WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No Rows Affected");

                    }
                }
            }
            catch (Exception)
            {
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }

    private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM TrainingProgram WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }

    }
       }