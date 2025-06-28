using Person.Api.EndPoints;

namespace Person.Api;

public static class EndpointRegistration
{
    public static void RegisterEndpoints(this WebApplication app)
    {
        app.MapPersonEndpoints();


    }
}





