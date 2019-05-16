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

    public class OrderTest
    {
        [Fact]
        public async Task Test_Get_All_Orders()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Order");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var orderList = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orderList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Order()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Order/3");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(3, order.CustomerId);
                Assert.Equal(3, order.PaymentTypeId);


                Assert.NotNull(order);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_Order_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Order/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        [Fact]
        public async Task Test_Create_And_Delete_Order()
        {

            using (var client = new APIClientProvider().Client)
            {
                Order orderOne = new Order
                {
                    CustomerId = 3,
                    PaymentTypeId = 3
                };
                var orderOneAsJSON = JsonConvert.SerializeObject(orderOne);


                var response = await client.PostAsync(
                    "api/Order",
                    new StringContent(orderOneAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newOrder = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(3, newOrder.CustomerId);
                Assert.Equal(3, newOrder.PaymentTypeId);


                var deleteResponse = await client.DeleteAsync($"api/Order/{newOrder.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Order_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/Orders/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Order()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Order modifiedOrder = new Order
                {
                    CustomerId = 3,
                    PaymentTypeId = 3,

                };
                var modifiedOrderAsJSON = JsonConvert.SerializeObject(modifiedOrder);

                var response = await client.PutAsync(
                    "api/Order/2",
                    new StringContent(modifiedOrderAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getOrder = await client.GetAsync("api/Order/2");
                getOrder.EnsureSuccessStatusCode();

                string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                Order newOrder = JsonConvert.DeserializeObject<Order>(getOrderBody);

                Assert.Equal(HttpStatusCode.OK, getOrder.StatusCode);
                Assert.Equal(3, newOrder.CustomerId);
                Assert.Equal(3, newOrder.PaymentTypeId);


            }
        }
    }
}
