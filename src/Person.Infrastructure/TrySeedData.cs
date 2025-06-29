using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Person.Domain.Entities;

namespace Person.Infrastructure
{
    public static class TrySeedData
    {
        public async static Task SeedPersonTestingDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            try
            {
                var ctx = scope.ServiceProvider.GetRequiredService<PersonDbContext>();
                if (await ctx.Database.EnsureCreatedAsync())
                {
                    // Seed PersonTypes
                    if (!ctx.PersonTypes.Any())
                    {
                        ctx.PersonTypes.AddRange(
                            new PersonType { PersonTypeId = 1, Description = "Teacher" },
                            new PersonType { PersonTypeId = 2, Description = "Student" }
                        );
                        await ctx.SaveChangesAsync();
                    }

                    // Seed Persons
                    if (!ctx.Persons.Any())
                    {
                        ctx.Persons.AddRange(
                            new Domain.Entities.Person { PersonId = 1, Name = "John Doe", Age = 30, PersonTypeId = 1 },
                            new Domain.Entities.Person { PersonId = 2, Name = "Jane Smith", Age = 25, PersonTypeId = 2 },
                            new Domain.Entities.Person { PersonId = 3, Name = "Alice Johnson", Age = 28, PersonTypeId = 1 },
                            new Domain.Entities.Person { PersonId = 4, Name = "Bob Brown", Age = 22, PersonTypeId = 2 },
                            new Domain.Entities.Person { PersonId = 5, Name = "Carol White", Age = 35, PersonTypeId = 1 },
                            new Domain.Entities.Person { PersonId = 6, Name = "David Black", Age = 20, PersonTypeId = 2 },
                            new Domain.Entities.Person { PersonId = 7, Name = "Eve Green", Age = 27, PersonTypeId = 1 },
                            new Domain.Entities.Person { PersonId = 8, Name = "Frank Blue", Age = 23, PersonTypeId = 2 },
                            new Domain.Entities.Person { PersonId = 9, Name = "Grace Red", Age = 32, PersonTypeId = 1 },
                            new Domain.Entities.Person { PersonId = 10, Name = "Henry Yellow", Age = 21, PersonTypeId = 2 },
                            new Domain.Entities.Person { PersonId = 11, Name = "Henry Blue", Age = 21, PersonTypeId = 2 }
                        );
                        await ctx.SaveChangesAsync();
                    }
                }
                // else block for migration is commented out, keep or remove as needed
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Migration from TrySeeding data: " + ex.Message);
            }
        }

        public async static Task SeedOnlyPersonTypesDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            try
            {
                var ctx = scope.ServiceProvider.GetRequiredService<PersonDbContext>();
                if (await ctx.Database.EnsureCreatedAsync())
                {
                    // Seed PersonTypes
                    if (!ctx.PersonTypes.Any())
                    {
                        ctx.PersonTypes.AddRange(
                            new PersonType { PersonTypeId = 1, Description = "Teacher" },
                            new PersonType { PersonTypeId = 2, Description = "Student" }
                        );
                        await ctx.SaveChangesAsync();
                    }
                }
                // else block for migration is commented out, keep or remove as needed
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Migration from TrySeeding data: " + ex.Message);
            }
        }
    }
}
