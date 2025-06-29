using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Person.Contracts;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, PersonResponse?>
{
    private readonly PersonDbContext _db;
    private readonly ILogger<GetPersonByIdQueryHandler> _logger;

    public GetPersonByIdQueryHandler(PersonDbContext db, ILogger<GetPersonByIdQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PersonResponse?> Handle(GetPersonByIdQuery query, CancellationToken ct)
    {
        _logger.LogInformation("Handling GetPersonByIdQuery for PersonId: {PersonId}", query.Id);

        var person = await _db.Persons
            .Include(p => p.PersonType)
            .FirstOrDefaultAsync(p => p.PersonId == query.Id, ct);

        if (person is null)
        {
            _logger.LogWarning("Person not found. PersonId: {PersonId}", query.Id);
            return null;
        }

        var response = new PersonResponse(
            person.PersonId,
            person.Name,
            person.Age,
            person.PersonTypeId,
            person.PersonType is not null
                ? new PersonTypeResponse(person.PersonType.PersonTypeId, person.PersonType.Description)
                : null
        );

        _logger.LogInformation("Person found: {@Person}", response);

        return response;
    }
}
