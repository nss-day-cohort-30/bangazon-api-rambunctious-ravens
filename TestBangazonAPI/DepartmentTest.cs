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
                var response = await client.GetAsync("api/Department/7");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<Department>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Nerd zone", department.Name);
                Assert.Equal(2, department.Budget);


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


        //[Fact]
        //public async Task Test_Create_And_Delete_Department()
        //{

        //    using (var client = new APIClientProvider().Client)
        //    {
        //        Department departmentOne = new Department
        //        {
        //            Name = "Marketing",
        //            Budget = 10

        //        };
        //        var departmentOneAsJSON = JsonConvert.SerializeObject(departmentOne);


        //        var response = await client.PostAsync(
        //            "api/Department",
        //            new StringContent(departmentOneAsJSON, Encoding.UTF8, "application/json")
        //        );

        //        response.EnsureSuccessStatusCode();

        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        var newDepartment = JsonConvert.DeserializeObject<Department>(responseBody);

        //        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //        Assert.Equal("Marketing", newDepartment.Name);
        //        Assert.Equal(10, newDepartment.Budget);


        //        var deleteResponse = await client.DeleteAsync($"api/Department/{newDepartment.Id}");
        //        deleteResponse.EnsureSuccessStatusCode();
        //        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        //    }
        //}

        //[Fact]
        //public async Task Test_Delete_NonExistent_Department_Fails()
        //{
        //    using (var client = new APIClientProvider().Client)
        //    {
        //        var deleteResponse = await client.DeleteAsync("/api/Departments/600000");

        //        Assert.False(deleteResponse.IsSuccessStatusCode);
        //        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        //    }
        //}

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
                    "api/Department/12",
                    new StringContent(modifiedDepartmentAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getDepartment = await client.GetAsync("api/Department/12");
                getDepartment.EnsureSuccessStatusCode();

                string getDepartmentBody = await getDepartment.Content.ReadAsStringAsync();
                Department newDepartment = JsonConvert.DeserializeObject<Department>(getDepartmentBody);

                Assert.Equal(HttpStatusCode.OK, getDepartment.StatusCode);
                Assert.Equal(newDepartmentName, newDepartment.Name);


            }
        }
    }
}
