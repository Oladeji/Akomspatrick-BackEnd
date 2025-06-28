namespace  Person.Integration.Tests.Base 
{
    public abstract class BaseIntegrationTests : IClassFixture<IntegrationTestWebAppFactory>
    {
        public readonly HttpClient _httpClient;

       // public readonly string _baseUrl = $"http://localhost:5007/{Endpoints.APIBase}/";
        public readonly string _baseUrl = $"http://localhost:5007/api/";

        public BaseIntegrationTests(IntegrationTestWebAppFactory factory)
        {
           _httpClient = factory.CreateClient();

        }
    }

}
