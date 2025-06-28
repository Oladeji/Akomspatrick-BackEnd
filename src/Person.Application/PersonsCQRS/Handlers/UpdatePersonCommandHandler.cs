using Person.Contracts;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand,int>
{
    private readonly PersonDbContext _db;
    public UpdatePersonCommandHandler(PersonDbContext db) => _db = db;

    public async Task<int> Handle(UpdatePersonCommand cmd, CancellationToken ct)
    {
        var person = await _db.Persons.FindAsync(new object[] { cmd.Id }, ct);
        if (person is null) throw new KeyNotFoundException();

        person.Name = cmd.Request.Name;
        person.Age = cmd.Request.Age;
        person.PersonTypeId = cmd.Request.PersonTypeId;
       return  await _db.SaveChangesAsync(ct);
    }
}
