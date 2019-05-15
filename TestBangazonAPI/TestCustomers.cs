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
    public class TestCustomers
    {
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customers");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Customer()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/customers/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Justina", customer.FirstName);
                Assert.Equal("Vickers", customer.LastName);
                Assert.NotNull(customer);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistant_Customer_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/customers/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                Customer customer = new Customer
                {
                    FirstName = "Steve",
                    LastName = "Brownlee",
                };
                var customerAsJSON = JsonConvert.SerializeObject(customer);


                var response = await client.PostAsync(
                    "/api/customers",
                    new StringContent(customerAsJSON, Encoding.UTF8, "application/json")
                );

                //response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                Customer newCustomer = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Steve", newCustomer.FirstName);
                Assert.Equal("Brownlee", newCustomer.LastName);

                var deleteResponse = await client.DeleteAsync($"/api/customers/{newCustomer.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Customer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/customers/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Customer()
        {
            // New last name to change to and test
            string newLastName = "Cronin-Test";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Customer modifiedSam = new Customer
                {
                    FirstName = "Sam",
                    LastName = newLastName,
                };

                var modifiedSamAsJSON = JsonConvert.SerializeObject(modifiedSam);

                var response = await client.PutAsync(
                    "/api/customers/2",
                    new StringContent(modifiedSamAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getSam = await client.GetAsync("/api/customers/2");
                getSam.EnsureSuccessStatusCode();

                string getSamBody = await getSam.Content.ReadAsStringAsync();
                Customer newSam = JsonConvert.DeserializeObject<Customer>(getSamBody);

                Assert.Equal(HttpStatusCode.OK, getSam.StatusCode);
                Assert.Equal(newLastName, newSam.LastName);
            }
        }
    }
}

