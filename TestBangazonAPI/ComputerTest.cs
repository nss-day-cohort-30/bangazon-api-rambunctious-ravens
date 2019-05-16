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
                var response = await client.GetAsync("api/Computer/4");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var computer = JsonConvert.DeserializeObject<Computer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("AlienWare", computer.Make);
                Assert.Equal("Dell", computer.Manufacturer);
               

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
            DateTime purchaseDate = DateTime.Now;
            DateTime decomissionDate = DateTime.Now;

            using (var client = new APIClientProvider().Client)
            {
                Computer computerOne = new Computer
                {
                    Make = "Goddard",
                    Manufacturer = "Lenovo",
                    PurchaseDate = purchaseDate,
                    DecomissionDate = decomissionDate
                };
                var computerOneAsJSON = JsonConvert.SerializeObject(computerOne);


                var response = await client.PostAsync(
                    "api/Computer",
                    new StringContent(computerOneAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newComputer = JsonConvert.DeserializeObject<Computer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Goddard", newComputer.Make);
                Assert.Equal("Lenovo", newComputer.Manufacturer);
                Assert.Equal(purchaseDate, newComputer.PurchaseDate);
                Assert.Equal(decomissionDate, newComputer.DecomissionDate);


                var deleteResponse = await client.DeleteAsync($"api/Computer/{newComputer.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Computer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/Computers/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Computer()
        {
            DateTime purchaseDate = DateTime.Now;
            DateTime decomissionDate = DateTime.Now;

            // New computer make, to change to and test
            string newComputerMake = "Dell";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Computer modifiedComputer = new Computer
                {
                    Make = newComputerMake,
                    Manufacturer = "Lenovo",
                    PurchaseDate = purchaseDate,
                    DecomissionDate = decomissionDate
                
                };
                var modifiedComputerAsJSON = JsonConvert.SerializeObject(modifiedComputer);

                var response = await client.PutAsync(
                    "api/Computer/1",
                    new StringContent(modifiedComputerAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getComputer = await client.GetAsync("api/Computer/1");
                getComputer.EnsureSuccessStatusCode();

                string getComputerBody = await getComputer.Content.ReadAsStringAsync();
                Computer newComputer = JsonConvert.DeserializeObject<Computer>(getComputerBody);

                Assert.Equal(HttpStatusCode.OK, getComputer.StatusCode);
                Assert.Equal(newComputerMake, newComputer.Make);
                

            }
        }
    }
}
