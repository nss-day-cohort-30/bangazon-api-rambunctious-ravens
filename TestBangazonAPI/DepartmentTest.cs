using Newtonsoft.Json;
using BangazonAPI.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System;

namespace TestBangazonAPI
{

    public class DepartmentTest
    {
        [Fact]
        public async Task Test_Get_All_Departments()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var departmentList = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departmentList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Department()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<Department>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Information Technology", department.Name);
                Assert.Equal(1000, department.Budget);


                Assert.NotNull(department);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_Department_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Department()
        {

            // New department make, to change to and test
            string newDepartmentName = "Test";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Department modifiedDepartment = new Department
                {
                    Name = newDepartmentName,
                    Budget = 10
                };
                var modifiedDepartmentAsJSON = JsonConvert.SerializeObject(modifiedDepartment);

                var response = await client.PutAsync(
                    "api/Department/2",
                    new StringContent(modifiedDepartmentAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getDepartment = await client.GetAsync("api/Department/2");
                getDepartment.EnsureSuccessStatusCode();

                string getDepartmentBody = await getDepartment.Content.ReadAsStringAsync();
                Department newDepartment = JsonConvert.DeserializeObject<Department>(getDepartmentBody);

                Assert.Equal(HttpStatusCode.OK, getDepartment.StatusCode);
                Assert.Equal(newDepartmentName, newDepartment.Name);


            }
        }
    }
}
