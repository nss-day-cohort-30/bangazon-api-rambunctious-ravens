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

    public class TestTrainingPogram
    {
        [Fact]
        public async Task Test_Get_All_TrainingPrograms()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/trainingprogram");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingProgramList = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingProgramList.Count > 0);
            }
        }


        [Fact]
        public async Task Test_Get_Single_TrainingProgram()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/trainingprogram/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingprogram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("PowerPoint", trainingprogram.Name);
                Assert.Equal(50, trainingprogram.MaxAttendees);
                Assert.NotNull(trainingprogram);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistent_TrainingProgram_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/trainingprogram/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }




        [Fact]
        public async Task Test_Create_And_Delete_TrainingProgram()
        {
            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;

            using (var client = new APIClientProvider().Client)
            {
                TrainingProgram safety = new TrainingProgram
                {
                    Name = "safety",
                    StartDate = startdate,
                    EndDate = enddate,
                    MaxAttendees = 50
                };
                var safetyAsJSON = JsonConvert.SerializeObject(safety);


                var response = await client.PostAsync(
                    "api/trainingprogram",
                    new StringContent(safetyAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newSafety = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("safety", newSafety.Name);
                Assert.Equal(startdate, newSafety.StartDate);
                Assert.Equal(enddate, newSafety.EndDate);
                Assert.Equal(50, newSafety.MaxAttendees);


                var deleteResponse = await client.DeleteAsync($"api/trainingprogram/{newSafety.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_TrainingProgram_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/trainingprogram/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_TrainingProgram()
        {
            // New training program name to change to and test

            string newName = "PowerPoint";
             DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;

            using (var client = new APIClientProvider().Client)
            {
                TrainingProgram modifiedProgram = new TrainingProgram
                {
                    Name = newName,
                    StartDate = startdate,
                    EndDate = enddate,
                    MaxAttendees = 50
                };
                var modifiedTrainingProgramAsJSON = JsonConvert.SerializeObject(modifiedProgram);

                var response = await client.PutAsync(
                    "api/trainingprogram/1",
                    new StringContent(modifiedTrainingProgramAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                
                var getEditTraining = await client.GetAsync("api/trainingprogram/1");
                getEditTraining.EnsureSuccessStatusCode();

                string getTrainingProgramBody = await getEditTraining.Content.ReadAsStringAsync();
                TrainingProgram newTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(getTrainingProgramBody);

                Assert.Equal(HttpStatusCode.OK, getEditTraining.StatusCode);
                //Assert.Equal(newName, newTrainingProgram.Name);
            }
        }
    }
}