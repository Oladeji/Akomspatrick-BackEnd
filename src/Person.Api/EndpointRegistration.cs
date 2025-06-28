using Person.Api.EndPoints;
using System.Runtime.Intrinsics.Arm;

namespace Person.Api;

public static class EndpointRegistration
{
    public static void RegisterEndpoints(this WebApplication app)
    {
        app.MapPersonEndpoints();


    }
}

public static class Endpoints
{
    public const string APIBase = "/api";
    public static class Persons
    {
        public const string Controller = "persons";
        public const string Get = $"{APIBase}/{Controller}";
        public const string GetById = $"{APIBase}/{Controller}/{{id:int}}";
        public const string Create = $"{APIBase}/{Controller}";
        public const string Update = $"{APIBase}/{Controller}/{{id:int}}";
        public const string Delete = $"{APIBase}/{Controller}/{{id:int}}";
        public const string GetAllTypes= $"{APIBase}/{Controller}/persontypes";


    }
}



