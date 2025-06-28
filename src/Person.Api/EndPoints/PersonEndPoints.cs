
using Microsoft.EntityFrameworkCore;
using Person.Api.Models; // Adjust namespace as needed

namespace Person.Api.EndPoints
{
    public static class PersonEndPoints
    {
        public static void MapPersonEndpoints(this IEndpointRouteBuilder app)
        {
            // Get all persons
            app.MapGet("/api/persons", async (PersonDbContext db) =>
                //await db.Persons.Include(p => p.PersonType).ToListAsync());
            await db.Persons.ToListAsync());

            // Get person by ID
            app.MapGet("/api/persons/{id:int}", async (int id, PersonDbContext db) =>
            {
                var person = await db.Persons.Include(p => p.PersonType)
                                             .FirstOrDefaultAsync(p => p.PersonId == id);
                return person is not null ? Results.Ok(person) : Results.NotFound();
            });

            // Create a new person
            app.MapPost("/api/persons", async (Models.Person person, PersonDbContext db) =>
            {
                db.Persons.Add(person);
                await db.SaveChangesAsync();
                return Results.Created($"/api/persons/{person.PersonId}", person);
            });

            // Update a person
            app.MapPut("/api/persons/{id:int}", async (int id, Models.Person updatedPerson, PersonDbContext db) =>
            {
                var person = await db.Persons.FindAsync(id);
                if (person is null) return Results.NotFound();

                person.Name = updatedPerson.Name;
                person.Age = updatedPerson.Age;
                person.PersonTypeId = updatedPerson.PersonTypeId;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // Delete a person
            app.MapDelete("/api/persons/{id:int}", async (int id, PersonDbContext db) =>
            {
                var person = await db.Persons.FindAsync(id);
                if (person is null) return Results.NotFound();

                db.Persons.Remove(person);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // Get all person types
            app.MapGet("/api/persontypes", async (PersonDbContext db) =>
                await db.PersonTypes.ToListAsync());
        }
    }
}
