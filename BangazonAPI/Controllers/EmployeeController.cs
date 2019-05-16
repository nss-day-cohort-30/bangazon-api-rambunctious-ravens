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
    public class EmployeeController : ControllerBase
    {
        //This controller was made by Justina and Sam
        //All of our fetch call methods for employees are created here
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
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
        //Using join on sql statement to join department and computer to employees
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, FirstName, LastName, IsSuperVisor, d.Name, d.Budget, c.Manufacturer, c.Make, c.PurchaseDate FROM Employee e
                                            JOIN Department d ON e.DepartmentId = d.Id
                                            JOIN ComputerEmployee ce ON e.Id = ce.EmployeeId
                                            JOIN Computer c ON c.Id = ce.ComputerId";



                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            },
                            Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            }
                        };

                        employees.Add(employee);
                    }

                    reader.Close();

                    return Ok(employees);
                }
            }



        }

        // GET api/values/5
        //Get a single item
        //Using join on sql statement to join department and computer to employees
        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, FirstName, LastName, IsSuperVisor, d.Name, d.Budget, c.Manufacturer, c.Make, c.PurchaseDate FROM Employee e
                                        JOIN Department d ON e.DepartmentId = d.Id
                                        JOIN ComputerEmployee ce ON e.Id = ce.EmployeeId
                                        JOIN Computer c ON c.Id = ce.ComputerId
                                        WHERE e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Employee employee = null;
                    if (reader.Read())
                    {
                        //creating the new object with the joined tables
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            },
                            Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            }
                        };
                    }
                    //checks to see if employee with an id we want to get already exists,
                    //if it does not we are returned with a status of not found
                    if (!EmployeeExists(id))
                    {
                        return NotFound();
                    }
                    reader.Close();
                    return Ok(employee);
                }

            }
        }

        // POST api/values
        //creating employee object to post to db
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"
                        INSERT INTO Employee (FirstName, LastName, IsSuperVisor, DepartmentId)
                        OUTPUT INSERTED.Id
                        VALUES (@FirstName, @LastName, @IsSuperVisor, @DepartmentId)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@IsSuperVisor", employee.IsSuperVisor));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentId", employee.DepartmentId));

                    //creates new id
                    int newId = (int)await cmd.ExecuteScalarAsync();
                    employee.Id = newId;
                    return CreatedAtRoute("GetEmployee", new { id = newId }, employee);

                }
            }
        }

        // PUT api/values/5
        //Sql statment to edit object
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        UPDATE Employee
                        SET FirstName = @FirstName,
                            LastName = @LastName,
                            IsSuperVisor = @IsSuperVisor,
                            DepartmentId = @DepartmentId
                        WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@IsSupervisor", employee.IsSuperVisor));
                        cmd.Parameters.Add(new SqlParameter("@DepartmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        //returns our 404 if the object with the requested id does not exist
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
                //checks to see if employee with an id we want to get already exists,
                //if it does not we are returned with a status of not found
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool EmployeeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Employee WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
