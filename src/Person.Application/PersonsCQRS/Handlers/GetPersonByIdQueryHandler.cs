using Microsoft.EntityFrameworkCore;
using Person.Contracts;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, PersonResponse?>
{
    private readonly PersonDbContext _db;
    public GetPersonByIdQueryHandler(PersonDbContext db) => _db = db;

    public async Task<PersonResponse?> Handle(GetPersonByIdQuery query, CancellationToken ct)
    {
        var p = await _db.Persons.Include(p => p.PersonType)
                                 .FirstOrDefaultAsync(p => p.PersonId == query.Id, ct);
        if (p is null) return null;
        return new PersonResponse(
            p.PersonId,
            p.Name,
            p.Age,
            p.PersonTypeId,
            p.PersonType is not null
                ? new PersonTypeResponse(p.PersonType.PersonTypeId, p.PersonType.Description)
                : null
        );
    }
}
