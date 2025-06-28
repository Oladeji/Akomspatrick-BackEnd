using System.Net;
using System.Net.Http.Json;
using Person.Contracts;
using Person.Integration.Tests.Base;

namespace Person.Integration.Tests.Endpoints
{
    public class PersonEndpointsTests : BaseIntegrationTests
    {
        // Static field to hold the loaded person types
        private  List<PersonTypeResponse>? _personTypes;

        public PersonEndpointsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
         
            //if (_personTypes == null)
            //{
            //    _personTypes = GetPersonTypesFromDbAsync().GetAwaiter().GetResult();
            //}
        }

        private async Task<List<PersonTypeResponse>> GetPersonTypesFromDbAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}persons/persontypes");//.GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var types = response.Content.ReadFromJsonAsync<List<PersonTypeResponse>>().GetAwaiter().GetResult();
            return types ?? new List<PersonTypeResponse>();
        }




        [Fact]
        public async Task GetAllPersons_ReturnsOkAndList()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}persons");
            response.EnsureSuccessStatusCode();
            var persons = await response.Content.ReadFromJsonAsync<List<PersonResponse>>();
            Assert.NotNull(persons);
        }
        private  async Task <CreatePersonRequest> GenerateFakePerson()
        {
            var random = new Random();
            _personTypes = await GetPersonTypesFromDbAsync(); ;
            var randomPersonTypeId = _personTypes[random.Next(_personTypes.Count)].PersonTypeId;
            var fakePersonGenerator = new AutoBogus.AutoFaker<CreatePersonRequest>()
                .RuleFor(x => x.PersonTypeId, f => f.IndexGlobal + 1) // Ensure unique PersonTypeId
                .RuleFor(x => x.Age, _ => random.Next(1, 101)) // Age between 1 and 100
                .RuleFor(x => x.Name, f => f.Person.FirstName+ ""+  f.Person.LastName);

            return fakePersonGenerator.Generate();
        }
        [Fact]
        public async Task GetPersonById_ReturnsOkOrNotFound()
        {
    
            var fakePersonRequest= await GenerateFakePerson();

          
            var createResponse = await _httpClient.PostAsJsonAsync($"{_baseUrl}persons", fakePersonRequest);
            var created = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

            // Test found
            var response = await _httpClient.GetAsync($"{_baseUrl}persons/{created.PersonId}");
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);

            // Test not found
            var notFoundResponse = await _httpClient.GetAsync($"{_baseUrl}persons/999999");
            Assert.Equal(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
        }

        [Fact]
        public async Task CreatePerson_ReturnsCreated()
        {
            // This test fails except when run alone, due to the random nature of the data generation.
            // This is a known issue with the random data generation, which can lead to conflicts in the database.
            // To ensure consistent results, we can use a fixed seed for the random number generator or mock the data generation.
            // or possible the databe is not cleaned up properly between tests.
            // will work on it again afater adding telemetrics
            var request = await GenerateFakePerson();

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}persons", request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var person = await response.Content.ReadFromJsonAsync<PersonResponse>();
            Assert.NotNull(person);
        }

        [Fact]
        public async Task CreatePerson_ReturnsBadRequest_OnInvalidData()
        {
           // var request = GenerateFakePerson();
            // Set invalid data for testing
           var request = new CreatePersonRequest(
                "", // Empty name
                -5, // Invalid age
                999 // Non-existing PersonTypeId
            );
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}persons", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_ReturnsNotFoundWhenDataDoesNotExist()
        {

            var updateRequest = new UpdatePersonRequest(
                "Updated Name",1, 1);
    
           // Update non-existing
            var notFoundResponse = await _httpClient.PutAsJsonAsync($"{_baseUrl}persons/999999", updateRequest);
            Assert.True(notFoundResponse.StatusCode == HttpStatusCode.NotFound || notFoundResponse.StatusCode == HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task UpdatePerson_ReturnsNoContent()
        {
            // Create a person
            var createRequest = await GenerateFakePerson();
            var createResponse = await _httpClient.PostAsJsonAsync($"{_baseUrl}persons", createRequest);
            var created = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

            // Update existing
            var updateRequest = new UpdatePersonRequest(
                "Updated Name", created.Age, created.PersonTypeId);


            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}persons/{created.PersonId}", updateRequest);
            Assert.True(response.StatusCode == HttpStatusCode.NoContent );

            }

        [Fact]
        public async Task DeletePerson_ReturnsNoContentOrNotFound()
        {
            // Create a person
            var createRequest = await GenerateFakePerson();
            var createResponse = await _httpClient.PostAsJsonAsync($"{_baseUrl}persons", createRequest);
            var created = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

            // Delete existing
            var response = await _httpClient.DeleteAsync($"{_baseUrl}persons/{created.PersonId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Delete non-existing
            var notFoundResponse = await _httpClient.DeleteAsync($"{_baseUrl}persons/999999");
            Assert.Equal(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
        }

        [Fact]
        public async Task GetAllPersonTypes_ReturnsOkAndList()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}persons/persontypes");
            response.EnsureSuccessStatusCode();
            var types = await response.Content.ReadFromJsonAsync<List<PersonTypeResponse>>();
            Assert.NotNull(types);
        }
    }
}
