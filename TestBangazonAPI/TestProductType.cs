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

    public class TestProductType
    {
        [Fact]
        public async Task Test_Product_Type_Get_All()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/producttype");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var ProductTypeList= JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(ProductTypeList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Product_Type()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/producttype/2");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var productType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Book", productType.Name);
                Assert.NotNull(productType);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistant_ProductType_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/producttype/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        [Fact]
        public async Task Test_Create_And_Delete_Product_Type()
        {
            using (var client = new APIClientProvider().Client)
            {
                ProductType raft = new ProductType
                {
                    Name = "raft"
                };
                var raftAsJson = JsonConvert.SerializeObject(raft);


                var response = await client.PostAsync(
                    "/api/producttype",
                    new StringContent(raftAsJson, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newRaft = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("raft", newRaft.Name);


                var deleteResponse = await client.DeleteAsync($"/api/producttype/{newRaft.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Product_Type()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/producttype/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Product_Type()
        {
            // New last name to change to and test
            string newProductTypeName = "houseBoat";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                ProductType modiefiedRaft = new ProductType
                {
                    Name = newProductTypeName,
                };

                var modiefiedAmexAsJSON = JsonConvert.SerializeObject(modiefiedRaft);

                var response = await client.PutAsync(
                    "/api/producttype/3",
                    new StringContent(modiefiedAmexAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getNiall = await client.GetAsync("/api/producttype/3");
                getNiall.EnsureSuccessStatusCode();

                string getNiallBody = await getNiall.Content.ReadAsStringAsync();
                ProductType newNiall = JsonConvert.DeserializeObject<ProductType>(getNiallBody);

                Assert.Equal(HttpStatusCode.OK, getNiall.StatusCode);
                Assert.Equal(newProductTypeName, newNiall.Name);
            }
        }
    }
}