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
                var response = await client.GetAsync("api/product/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        [Fact]
        public async Task Test_Create_And_Delete_Student()
        {
            using (var client = new APIClientProvider().Client)
            {
                Product BookOnPlants = new Product
                {
                    ProductTypeId = 1,
                    CustomerId = 1,
                    Description = "A book on edible plants",
                    Price = 20,
                    Title = "Do eat these",
                    Quantity = 40,
                };
                var BookOnPlantsAsJSON = JsonConvert.SerializeObject(BookOnPlants);


                var response = await client.PostAsync(
                    "api/product",
                    new StringContent(BookOnPlantsAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newBookOnPlants = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("A book on edible plants", newBookOnPlants.Description);
                Assert.Equal(20, newBookOnPlants.Price);
                Assert.Equal("Do eat these", newBookOnPlants.Title);
                Assert.Equal(40, newBookOnPlants.Quantity);


                var deleteResponse = await client.DeleteAsync($"api/product/{newBookOnPlants.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Student_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/product/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Product()
        {
            // New last name to change to and test
            string newDescription = "The Bee is no longer large";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Product modifiedProduct= new Product
                {
                    CustomerId = 1,
                    ProductTypeId = 1,
                    Title = "A Big Bee",
                    Price = 1000,
                    Description = newDescription ,
                    Quantity = 3,
                };
                var modifiedProductJSON = JsonConvert.SerializeObject(modifiedProduct);

                var response = await client.PutAsync(
                    "/api/product/1",
                    new StringContent(modifiedProductJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getProduct = await client.GetAsync("/api/product/1");
                getProduct.EnsureSuccessStatusCode();

                string getProductBody = await getProduct.Content.ReadAsStringAsync();
                Product newProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

                Assert.Equal(HttpStatusCode.OK, getProduct.StatusCode);
                Assert.Equal(newDescription, newProduct.Description);
            }
        }
    }
}