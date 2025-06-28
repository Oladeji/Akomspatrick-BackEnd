using Microsoft.EntityFrameworkCore;
using Person.Contracts;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class GetAllPersonTypesQueryHandler : IRequestHandler<GetAllPersonTypesQuery, List<PersonTypeResponse>>
{
    private readonly PersonDbContext _db;
    public GetAllPersonTypesQueryHandler(PersonDbContext db) => _db = db;

    public async Task<List<PersonTypeResponse>> Handle(GetAllPersonTypesQuery query, CancellationToken ct)
    {
        var types = await _db.PersonTypes.ToListAsync(ct);
        return types.Select(t => new PersonTypeResponse(t.PersonTypeId, t.Description)).ToList();
    }
}
