using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Person.Contracts;
using Person.Shared.CQRS;

namespace Person.Api.EndPoints
{
    public static class PersonEndPoints
    {
        public static void MapPersonEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet(Endpoints.Persons.Get, GetAllPersonsAsync)
                   .WithName(Endpoints.Persons.Get + "GetAllPersonsAsync")
                   .Produces<List<PersonResponse>>(StatusCodes.Status200OK);

            app.MapGet(Endpoints.Persons.GetById, GetPersonByIdAsync)
                .WithName(Endpoints.Persons.GetById + "GetPersonByIdAsync")
                .Produces<PersonResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound, "application/problem+json");

            app.MapPost(Endpoints.Persons.Create, CreatePersonAsync)
                .WithName(Endpoints.Persons.Create + "CreatePersonAsync")
                .Produces<PersonResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json");

            app.MapPut(Endpoints.Persons.Update, UpdatePersonAsync)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<PersonResponse>(StatusCodes.Status200OK, "application/json")
                .ProducesProblem(StatusCodes.Status404NotFound, "application/problem+json")
                .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json");

            app.MapDelete(Endpoints.Persons.Delete, DeletePersonAsync)
                .WithName(Endpoints.Persons.Delete + "DeletePersonAsync")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound, "application/problem+json");

            app.MapGet(Endpoints.Persons.GetAllTypes, GetAllPersonTypesAsync)
                .WithName(Endpoints.Persons.GetAllTypes + "GetAllPersonTypesAsync")
                .Produces<List<PersonTypeResponse>>(StatusCodes.Status200OK, "application/json");
        }

        private static async Task<IResult> GetAllPersonsAsync(
            [FromServices] ISender sender,
            [FromServices] ILoggerFactory loggerFactory,
            CancellationToken ct)
        {
            var logger = loggerFactory.CreateLogger("GetAllPersonsAsync");
            logger.LogInformation("Getting all persons");
            var result = await sender.Send(new GetAllPersonsQuery(), ct);
            logger.LogInformation("Retrieved {Count} persons", (result as List<PersonResponse>)?.Count ?? 0);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetPersonByIdAsync(
            [FromServices] ISender sender,
            [FromRoute] int id,
            [FromServices] ILoggerFactory loggerFactory,
            CancellationToken ct)
        {
            var logger = loggerFactory.CreateLogger("GetPersonByIdAsync");
            logger.LogInformation("Getting person by id: {Id}", id);
            var result = await sender.Send(new GetPersonByIdQuery(id), ct);
            if (result is not null)
            {
                logger.LogInformation("Person found: {Id}", id);
                return Results.Ok(result);
            }
            else
            {
                logger.LogWarning("Person not found: {Id}", id);
                return Results.NotFound();
            }
        }

        private static async Task<IResult> CreatePersonAsync(
            CreatePersonRequest request,
            [FromServices] ISender sender,
            [FromServices] ILoggerFactory loggerFactory,
            CancellationToken ct)
        {
            var logger = loggerFactory.CreateLogger("CreatePersonAsync");
            logger.LogInformation("Creating person with Name: {Name}, Age: {Age}, PersonTypeId: {PersonTypeId}", request?.Name, request?.Age, request?.PersonTypeId);

            // Proper validation will be done for a full system 
            // use FluentValidation or similar libraries for complex validation
            if (request is null || request.Name.IsNullOrEmpty() || request.Age < 0 || request.PersonTypeId < 0)
            {
                logger.LogWarning("Invalid create person request: {@Request}", request);
                return Results.BadRequest("Request body cannot be null.");
            }

            var result = await sender.Send(new CreatePersonCommand(request), ct);
            logger.LogInformation("Person created with Id: {PersonId}", result.PersonId);
            return Results.Created($"{Endpoints.APIBase}/{Endpoints.Persons.Controller}/{result.PersonId}", result);
        }

        private static async Task<IResult> UpdatePersonAsync(
            int id,
            UpdatePersonRequest request,
            [FromServices] ISender sender,
            [FromServices] ILoggerFactory loggerFactory,
            CancellationToken ct)
        {
            var logger = loggerFactory.CreateLogger("UpdatePersonAsync");
            logger.LogInformation("Updating person with Id: {Id}, Name: {Name}, Age: {Age}, PersonTypeId: {PersonTypeId}", id, request?.Name, request?.Age, request?.PersonTypeId);
            try
            {
                await sender.Send(new UpdatePersonCommand(id, request), ct);
                logger.LogInformation("Person updated: {Id}", id);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                logger.LogWarning("Person not found for update: {Id}", id);
                return Results.NotFound();
            }
        }

        private static async Task<IResult> DeletePersonAsync(
            int id,
            [FromServices] ISender sender,
            [FromServices] ILoggerFactory loggerFactory,
            CancellationToken ct)
        {
            var logger = loggerFactory.CreateLogger("DeletePersonAsync");
            logger.LogInformation("Deleting person with Id: {Id}", id);
            try
            {
                await sender.Send(new DeletePersonCommand(id), ct);
                logger.LogInformation("Person deleted: {Id}", id);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                logger.LogWarning("Person not found for delete: {Id}", id);
                return Results.NotFound();
            }
        }

        private static async Task<IResult> GetAllPersonTypesAsync(
            [FromServices] ISender sender,
            [FromServices] ILoggerFactory loggerFactory,
            CancellationToken ct)
        {
            var logger = loggerFactory.CreateLogger("GetAllPersonTypesAsync");
            logger.LogInformation("Getting all person types");
            var result = await sender.Send(new GetAllPersonTypesQuery(), ct);
            logger.LogInformation("Retrieved {Count} person types", (result as List<PersonTypeResponse>)?.Count ?? 0);
            return Results.Ok(result);
        }
    }
}
