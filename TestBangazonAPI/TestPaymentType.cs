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
    public class TestPaymentTypes
    {
        [Fact]
        public async Task Test_Get_All_Payment_Types()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/PaymentType");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var paymenttypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymenttypes.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Payment_Type()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/paymenttype/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(1992, paymentType.AcctNumber);
                Assert.Equal("Amex", paymentType.Name);
                Assert.NotNull(paymentType);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistant_Payment_Type__Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/paymenttype/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_Payment_Type()
        {
            using (var client = new APIClientProvider().Client)
            {
                PaymentType amex = new PaymentType
                {
                    AcctNumber = 5678,
                    Name = "Amex",
                    CustomerId = 1
                };
                var amexAsJson = JsonConvert.SerializeObject(amex);


                var response = await client.PostAsync(
                    "/api/paymenttype",
                    new StringContent(amexAsJson, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newAmex = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(5678, newAmex.AcctNumber);
                Assert.Equal("Amex", newAmex.Name);


                var deleteResponse = await client.DeleteAsync($"/api/paymenttype/{newAmex.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }


        [Fact]
        public async Task Test_Delete_NonExistant_Payment_Type_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/PaymentType/999999");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Payment_Type()
        {
            // New last name to change to and test
            int newAcctNumber = 1992;

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                PaymentType modiefiedAmex = new PaymentType
                {
                    AcctNumber = newAcctNumber,
                    Name = "Amex",
                    CustomerId = 1
                };

                var modiefiedAmexAsJSON = JsonConvert.SerializeObject(modiefiedAmex);

                var response = await client.PutAsync(
                    "/api/paymenttype/1",
                    new StringContent(modiefiedAmexAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getNiall = await client.GetAsync("/api/paymenttype/1");
                getNiall.EnsureSuccessStatusCode();

                string getNiallBody = await getNiall.Content.ReadAsStringAsync();
                PaymentType newNiall = JsonConvert.DeserializeObject<PaymentType>(getNiallBody);

                Assert.Equal(HttpStatusCode.OK, getNiall.StatusCode);
                Assert.Equal(newAcctNumber, newNiall.AcctNumber);
            }
        }
    }
}

