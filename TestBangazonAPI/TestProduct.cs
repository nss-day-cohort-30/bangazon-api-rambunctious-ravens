using Newtonsoft.Json;
using BangazonAPI.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace TestBangazonAPI
{

    public class TestProducts
    {
        [Fact]
        public async Task Test_Get_All_Products()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/product");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var ProductList = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(ProductList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Product()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/product/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("A Big Bee", product.Title);
                Assert.Equal("This bee is huge", product.Description);
                Assert.NotNull(product);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistant_Product_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/product/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        [Fact]
        public async Task Test_Create_And_Delete_Student()
        {
            using (var client = new APIClientProvider().Client)
            {
                Product helen = new Product
                {
                    FirstName = "Helen",
                    LastName = "Chalmers",
                    CohortId = 1,
                    SlackHandle = "Helen Chalmers"
                };
                var helenAsJSON = JsonConvert.SerializeObject(helen);


                var response = await client.PostAsync(
                    "/student",
                    new StringContent(helenAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newHelen = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Helen", newHelen.FirstName);
                Assert.Equal("Chalmers", newHelen.LastName);
                Assert.Equal("Helen Chalmers", newHelen.SlackHandle);


                var deleteResponse = await client.DeleteAsync($"/student/{newHelen.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        //[Fact]
        //public async Task Test_Delete_NonExistent_Student_Fails()
        //{
        //    using (var client = new APIClientProvider().Client)
        //    {
        //        var deleteResponse = await client.DeleteAsync("/api/students/600000");

        //        Assert.False(deleteResponse.IsSuccessStatusCode);
        //        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        //    }
        //}

        //[Fact]
        //public async Task Test_Modify_Student()
        //{
        //    // New last name to change to and test
        //    string newLastName = "Williams-Spradlin";

        //    using (var client = new APIClientProvider().Client)
        //    {
        //        /*
        //            PUT section
        //         */
        //        Student modifiedKate = new Student
        //        {
        //            FirstName = "Kate",
        //            LastName = newLastName,
        //            CohortId = 1,
        //            SlackHandle = "@katerebekah"
        //        };
        //        var modifiedKateAsJSON = JsonConvert.SerializeObject(modifiedKate);

        //        var response = await client.PutAsync(
        //            "/student/1",
        //            new StringContent(modifiedKateAsJSON, Encoding.UTF8, "application/json")
        //        );
        //        response.EnsureSuccessStatusCode();
        //        string responseBody = await response.Content.ReadAsStringAsync();

        //        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        //        /*
        //            GET section
        //         */
        //        var getKate = await client.GetAsync("/student/1");
        //        getKate.EnsureSuccessStatusCode();

        //        string getKateBody = await getKate.Content.ReadAsStringAsync();
        //        Student newKate = JsonConvert.DeserializeObject<Student>(getKateBody);

        //        Assert.Equal(HttpStatusCode.OK, getKate.StatusCode);
        //        Assert.Equal(newLastName, newKate.LastName);
        //    }
        //}
    }
}