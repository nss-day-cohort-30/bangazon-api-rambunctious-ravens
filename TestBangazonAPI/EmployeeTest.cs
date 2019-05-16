using BangazonAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestBangazonAPI;
using Xunit;

namespace TestEmployeeExercisesAPI
{

    public class TestEmployees
    {
        [Fact]
        public async Task Test_Gert_All_Employees()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Employee");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var employeeList = JsonConvert.DeserializeObject<List<Employee>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employeeList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Employee()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Employee/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<Employee>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(employee);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_Employee_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Employee/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        [Fact]
        public async Task Test_Modify_Employee()
        {
            // New last name to change to and test
            string newLastName = "Spalding";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Employee modifiedKate = new Employee
                {
                    FirstName = "Kate",
                    LastName = newLastName,
                    DepartmentId = 2,
                    IsSuperVisor = true
                };
                var modifiedKateAsJSON = JsonConvert.SerializeObject(modifiedKate);

                var response = await client.PutAsync(
                    "api/Employee/2",
                    new StringContent(modifiedKateAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getKate = await client.GetAsync("api/Employee/2");
                getKate.EnsureSuccessStatusCode();

                string getKateBody = await getKate.Content.ReadAsStringAsync();
                Employee newKate = JsonConvert.DeserializeObject<Employee>(getKateBody);

                Assert.Equal(HttpStatusCode.OK, getKate.StatusCode);
                Assert.Equal(newLastName, newKate.LastName);
            }
        }
    }
}