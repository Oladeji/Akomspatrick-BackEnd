using Microsoft.Extensions.Logging;
using Person.Contracts;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, PersonResponse>
{
    private readonly PersonDbContext _db;
    private readonly ILogger<CreatePersonCommandHandler> _logger;

    public CreatePersonCommandHandler(PersonDbContext db, ILogger<CreatePersonCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PersonResponse> Handle(CreatePersonCommand cmd, CancellationToken ct)
    {
        _logger.LogInformation("Handling CreatePersonCommand: {@Request}", cmd.Request);

        var person = new Person.Domain.Entities.Person
        {
            Name = cmd.Request.Name,
            Age = cmd.Request.Age,
            PersonTypeId = cmd.Request.PersonTypeId
        };

        _db.Persons.Add(person);
        _logger.LogDebug("Added new person entity to context: {@Person}", person);

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Person created with Id: {PersonId}, Name: {Name}, Age: {Age}, PersonTypeId: {PersonTypeId}",
            person.PersonId, person.Name, person.Age, person.PersonTypeId);

        var response = new PersonResponse(
            person.PersonId,
            person.Name,
            person.Age,
            person.PersonTypeId,
            null
        );

        _logger.LogDebug("Returning PersonResponse: {@Response}", response);

        return response;
    }
}
