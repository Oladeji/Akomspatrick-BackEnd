using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Person.Contracts;
using Person.Infrastructure;
using Person.Shared.CQRS; // Adjust namespace as needed

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
        private static async Task<IResult> GetAllPersonsAsync([FromServices] ISender sender, CancellationToken ct)
        {
            var result = await sender.Send(new GetAllPersonsQuery(), ct);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetPersonByIdAsync( [FromServices] ISender sender, [FromRoute] int id, CancellationToken ct)
        {
            var result = await sender.Send(new GetPersonByIdQuery(id), ct);
            return result is not null
                ? Results.Ok(result)
                : Results.NotFound();
        }

        private static async Task<IResult> CreatePersonAsync(CreatePersonRequest request, [FromServices] ISender sender, CancellationToken ct)
        {
            var result = await sender.Send(new CreatePersonCommand(request), ct);
            return Results.Created($"{Endpoints.APIBase}/{Endpoints.Persons.Controller}/{result.PersonId}", result);
        }

        private static async Task<IResult> UpdatePersonAsync(int id, UpdatePersonRequest request, [FromServices] ISender sender, CancellationToken ct)
        {
            try
            {
                await sender.Send(new UpdatePersonCommand(id, request), ct);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        }

        private static async Task<IResult> DeletePersonAsync(int id, [FromServices] ISender sender, CancellationToken ct)
        {
            try
            {
                await sender.Send(new DeletePersonCommand(id), ct);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        }

        private static async Task<IResult> GetAllPersonTypesAsync([FromServices] ISender sender, CancellationToken ct)
        {
            var result = await sender.Send(new GetAllPersonTypesQuery(), ct);
            return Results.Ok(result);
        }
    }
}
