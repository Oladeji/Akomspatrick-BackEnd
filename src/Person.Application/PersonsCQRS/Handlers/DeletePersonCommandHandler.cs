using Person.Infrastructure;
using Person.Shared.CQRS;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand,int>
{
    private readonly PersonDbContext _db;
    public DeletePersonCommandHandler(PersonDbContext db) => _db = db;

    public async Task<int> Handle(DeletePersonCommand cmd, CancellationToken ct)
    {
        var person = await _db.Persons.FindAsync(new object[] { cmd.Id }, ct);
        if (person is null) throw new KeyNotFoundException();

        _db.Persons.Remove(person);
        return await _db.SaveChangesAsync(ct);
    }
}
