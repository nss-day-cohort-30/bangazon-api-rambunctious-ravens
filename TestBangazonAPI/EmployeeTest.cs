using Newtonsoft.Json;
using BangazonAPI.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using TestBangazonAPI;

namespace TestEmployeeExercisesAPI
{

    public class TestEmployees
    {
        [Fact]
        public async Task Test_Get_All_Employees()
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

        //[Fact]
        //public async Task Test_Get_Single_Employee()
        //{

        //    using (var client = new APIClientProvider().Client)
        //    {
        //        var response = await client.GetAsync("/employee/1");

        //        response.EnsureSuccessStatusCode();

        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        var employee = JsonConvert.DeserializeObject<Employee>(responseBody);

        //        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //        Assert.Equal("Sam", employee.FirstName);
        //        Assert.Equal("Cronin", employee.LastName);
        //        Assert.NotNull(employee);
        //    }
        //}

        //[Fact]
        //public async Task Test_Get_NonExitant_Employee_Fails()
        //{

        //    using (var client = new APIClientProvider().Client)
        //    {
        //        var response = await client.GetAsync("/employee/999999999");
        //        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //    }
        //}


        //[Fact]
        //public async Task Test_Create_And_Delete_Employee()
        //{
        //    using (var client = new APIClientProvider().Client)
        //    {
        //        Employee helen = new Employee
        //        {
        //            FirstName = "Helen",
        //            LastName = "Chalmers",
        //            CohortId = 1,
        //            SlackHandle = "Helen Chalmers"
        //        };
        //        var helenAsJSON = JsonConvert.SerializeObject(helen);


        //        var response = await client.PostAsync(
        //            "/employee",
        //            new StringContent(helenAsJSON, Encoding.UTF8, "application/json")
        //        );

        //        response.EnsureSuccessStatusCode();

        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        var newHelen = JsonConvert.DeserializeObject<Employee>(responseBody);

        //        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //        Assert.Equal("Helen", newHelen.FirstName);
        //        Assert.Equal("Chalmers", newHelen.LastName);
        //        Assert.Equal("Helen Chalmers", newHelen.SlackHandle);


        //        var deleteResponse = await client.DeleteAsync($"/employee/{newHelen.Id}");
        //        deleteResponse.EnsureSuccessStatusCode();
        //        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        //    }
        //}

        //[Fact]
        //public async Task Test_Delete_NonExistent_Employee_Fails()
        //{
        //    using (var client = new APIClientProvider().Client)
        //    {
        //        var deleteResponse = await client.DeleteAsync("/api/employees/600000");

        //        Assert.False(deleteResponse.IsSuccessStatusCode);
        //        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        //    }
        //}

        //[Fact]
        //public async Task Test_Modify_Employee()
        //{
        //    // New last name to change to and test
        //    string newLastName = "Williams-Spradlin";

        //    using (var client = new APIClientProvider().Client)
        //    {
        //        /*
        //            PUT section
        //         */
        //        Employee modifiedKate = new Employee
        //        {
        //            FirstName = "Kate",
        //            LastName = newLastName,
        //            CohortId = 1,
        //            SlackHandle = "@katerebekah"
        //        };
        //        var modifiedKateAsJSON = JsonConvert.SerializeObject(modifiedKate);

        //        var response = await client.PutAsync(
        //            "/employee/1",
        //            new StringContent(modifiedKateAsJSON, Encoding.UTF8, "application/json")
        //        );
        //        response.EnsureSuccessStatusCode();
        //        string responseBody = await response.Content.ReadAsStringAsync();

        //        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        //        /*
        //            GET section
        //         */
        //        var getKate = await client.GetAsync("/employee/1");
        //        getKate.EnsureSuccessStatusCode();

        //        string getKateBody = await getKate.Content.ReadAsStringAsync();
        //        Employee newKate = JsonConvert.DeserializeObject<Employee>(getKateBody);

        //        Assert.Equal(HttpStatusCode.OK, getKate.StatusCode);
        //        Assert.Equal(newLastName, newKate.LastName);
        //    }
        //}
    }
}