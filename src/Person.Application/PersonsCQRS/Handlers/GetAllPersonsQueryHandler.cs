using Microsoft.EntityFrameworkCore;
using Person.Contracts;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class GetAllPersonsQueryHandler : IRequestHandler<GetAllPersonsQuery, List<PersonResponse>>
{
    private readonly PersonDbContext _db;
    public GetAllPersonsQueryHandler(PersonDbContext db) => _db = db;

    public async Task<List<PersonResponse>> Handle(GetAllPersonsQuery query, CancellationToken ct)
    {
        var persons = await _db.Persons.Include(p => p.PersonType).ToListAsync(ct);
        return persons.Select(p => new PersonResponse(
            p.PersonId,
            p.Name,
            p.Age,
            p.PersonTypeId,
            p.PersonType is not null
                ? new PersonTypeResponse(p.PersonType.PersonTypeId, p.PersonType.Description)
                : null
        )).ToList();
    }
}
