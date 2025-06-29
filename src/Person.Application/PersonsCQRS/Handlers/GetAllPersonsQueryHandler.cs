using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Person.Contracts;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class GetAllPersonsQueryHandler : IRequestHandler<GetAllPersonsQuery, List<PersonResponse>>
{
    private readonly PersonDbContext _db;
    private readonly ILogger<GetAllPersonsQueryHandler> _logger;

    public GetAllPersonsQueryHandler(PersonDbContext db, ILogger<GetAllPersonsQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<PersonResponse>> Handle(GetAllPersonsQuery query, CancellationToken ct)
    {
        _logger.LogInformation("Handling GetAllPersonsQuery: {@Query}", query);

        var persons = await _db.Persons.Include(p => p.PersonType).ToListAsync(ct);

        _logger.LogInformation("Retrieved {Count} persons from database.", persons.Count);

        var responses = persons.Select(p => new PersonResponse(
            p.PersonId,
            p.Name,
            p.Age,
            p.PersonTypeId,
            p.PersonType is not null
                ? new PersonTypeResponse(p.PersonType.PersonTypeId, p.PersonType.Description)
                : null
        )).ToList();

        _logger.LogDebug("Person responses: {@Responses}", responses);

        return responses;
    }
}
