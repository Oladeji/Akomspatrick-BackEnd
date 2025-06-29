using Microsoft.Extensions.Logging;
using Person.Contracts;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class GetAllPersonTypesQueryHandler : IRequestHandler<GetAllPersonTypesQuery, List<PersonTypeResponse>>
{
    private readonly PersonDbContext _db;
    private readonly ILogger<GetAllPersonTypesQueryHandler> _logger;

    public GetAllPersonTypesQueryHandler(PersonDbContext db, ILogger<GetAllPersonTypesQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<PersonTypeResponse>> Handle(GetAllPersonTypesQuery query, CancellationToken ct)
    {
        _logger.LogInformation("Handling GetAllPersonTypesQuery: {@Query}", query);

        var types = _db.PersonTypes.Select(pt => new PersonTypeResponse(pt.PersonTypeId, pt.Description)).ToList();

        _logger.LogInformation("Retrieved {Count} person types from database.", types.Count);
        _logger.LogDebug("PersonTypeResponses: {@Types}", types);

        return types;
    }
}
