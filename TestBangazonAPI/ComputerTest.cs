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

    public class ComputerTest
    {
        [Fact]
        public async Task Test_Get_All_Computers()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Computer");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var computerList = JsonConvert.DeserializeObject<List<Computer>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computerList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Computer()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Computer/10");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var computer = JsonConvert.DeserializeObject<Computer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Gala", computer.Make);
                Assert.Equal("Apple", computer.Manufacturer);
               

                Assert.NotNull(computer);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_Computer_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Computer/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        [Fact]
        public async Task Test_Create_And_Delete_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                Computer helen = new Computer
                {
                    Make = "Goddard",
                    Manufacturer = "Lenovo",
                    PurchaseDate = 12/12/2020,
                    DecomissionDate = null
                };
                var helenAsJSON = JsonConvert.SerializeObject(helen);


                //            var response = await client.PostAsync(
                //                "/computer",
                //                new StringContent(helenAsJSON, Encoding.UTF8, "application/json")
                //            );

                //            response.EnsureSuccessStatusCode();

                //            string responseBody = await response.Content.ReadAsStringAsync();
                //            var newHelen = JsonConvert.DeserializeObject<Computer>(responseBody);

                //            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                //            Assert.Equal("Helen", newHelen.FirstName);
                //            Assert.Equal("Chalmers", newHelen.LastName);
                //            Assert.Equal("Helen Chalmers", newHelen.SlackHandle);


                //            var deleteResponse = await client.DeleteAsync($"/computer/{newHelen.Id}");
                //            deleteResponse.EnsureSuccessStatusCode();
                //            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
                //        }
                //    }

                //    [Fact]
                //    public async Task Test_Delete_NonExistent_Computer_Fails()
                //    {
                //        using (var client = new APIClientProvider().Client)
                //        {
                //            var deleteResponse = await client.DeleteAsync("/api/computers/600000");

                //            Assert.False(deleteResponse.IsSuccessStatusCode);
                //            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
                //        }
                //    }

                //    [Fact]
                //    public async Task Test_Modify_Computer()
                //    {
                //        // New last name to change to and test
                //        string newLastName = "Williams-Spradlin";

                //        using (var client = new APIClientProvider().Client)
                //        {
                //            /*
                //                PUT section
                //             */
                //            Computer modifiedKate = new Computer
                //            {
                //                FirstName = "Kate",
                //                LastName = newLastName,
                //                CohortId = 1,
                //                SlackHandle = "@katerebekah"
                //            };
                //            var modifiedKateAsJSON = JsonConvert.SerializeObject(modifiedKate);

                //            var response = await client.PutAsync(
                //                "/computer/1",
                //                new StringContent(modifiedKateAsJSON, Encoding.UTF8, "application/json")
                //            );
                //            response.EnsureSuccessStatusCode();
                //            string responseBody = await response.Content.ReadAsStringAsync();

                //            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                //            /*
                //                GET section
                //             */
                //            var getKate = await client.GetAsync("/computer/1");
                //            getKate.EnsureSuccessStatusCode();

                //            string getKateBody = await getKate.Content.ReadAsStringAsync();
                //            Computer newKate = JsonConvert.DeserializeObject<Computer>(getKateBody);

                //            Assert.Equal(HttpStatusCode.OK, getKate.StatusCode);
                //            Assert.Equal(newLastName, newKate.LastName);
                //        }
                //    }
            }
}
