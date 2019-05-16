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
        //this controller is in charge of all training program CRUD operations. Author: Ken Perkerwicz 
          
        private readonly IConfiguration _config;


        // method to get private variable public
        public TrainingProgramController(IConfiguration config)
        {
            _config = config;
        }

        // method to connect to SQL Database

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET all Training Programs method 
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            
            using (SqlConnection conn = Connection)
            {
                // connection open method 
                conn.Open();

                // cmd will create a command to use in SQL database

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * from TrainingProgram";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();


                    // new list of type trainingProgram  // 

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    
                    // date time variables. they suck. //
                    DateTime? StartDate = null;
                    DateTime? EndDate = null;


                    while (reader.Read())
                    {

                        //while there are still rows to be processed, create a new TrainingProgram instance and add each to the list 
                        //training programs 

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

                    // once that is completed, close the reader 
                    reader.Close();

                    //return the list of training programs // 
                    return Ok(trainingPrograms);
                }
            }
        }



        // GET api/trainingPrograms by ID 

        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get(int id)

        {// use sql connection //
            using (SqlConnection conn = Connection)

            {
                // open said connection 
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // command text selects rows where the id entered into the method is used to match up with the row 
                    cmd.CommandText = $@"Select Id, Name, StartDate, EndDate, MaxAttendees from TrainingProgram
                                          WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    //setting an initial null value to trainingProgram//

                    TrainingProgram trainingProgram = null;
                    if (reader.Read())
                    {

                        // if there are rows to be read, then create a new object in C# called trainingProgram //
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),

                        };

                    }

                    if (!TrainingProgramExists(id))
                        //if the training program (accessed by id) does not exist then a notFound is returned //
                    {
                        return NotFound();

                    };

                        reader.Close();

                    return Ok(trainingProgram);
                }
            }
          }
            
        



      //POST method 
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TrainingProgram trainingprogram)
    {
     using (SqlConnection conn = Connection)
    {
         conn.Open();
         using (SqlCommand cmd = conn.CreateCommand())
        {
               // insert these values into Training Program table in SQL 
               cmd.CommandText = $@"
              INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
                  OUTPUT INSERTED.Id
                 VALUES (@Name, @StartDate, @EndDate, @MaxAttendees) ";
                    // add new values into sql that are taken from the object training program that is passed into the method on top. 

              cmd.Parameters.Add(new SqlParameter("@Name", trainingprogram.Name));
                    cmd.Parameters.Add(new SqlParameter("@StartDate", trainingprogram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@EndDate", trainingprogram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingprogram.MaxAttendees));

                    trainingprogram.Id = (int) await cmd.ExecuteScalarAsync();

                    
            return CreatedAtRoute("GetTrainingProgram", new { id = trainingprogram.Id }, trainingprogram);
           }
       }
        }




        /// Update a particular training program by the Id
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TrainingProgram trainingprogram)
        {
            try
                // try catch to implement code unless it fails. if it does then go to catch. 

            {
                // put method updates the training program 
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
                        // take all of the params that are taken from the indiv training program object and put them in the @variables, then set the command text to store those variables to the left. 
                        cmd.Parameters.Add(new SqlParameter("@id", id));
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

        //DELETE trainingProgram by its ID

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


        // boolean to check if a training program exists by ID // 

    private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // SQL Select 
                    cmd.CommandText = "SELECT Id FROM TrainingProgram WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }

    }
       }