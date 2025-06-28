using Person.Contracts;
using Person.Domain.Entities;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, PersonResponse>
{
    private readonly PersonDbContext _db;
    public CreatePersonCommandHandler(PersonDbContext db) => _db = db;

    public async Task<PersonResponse> Handle(CreatePersonCommand cmd, CancellationToken ct)
    {
        var person = new Person.Domain.Entities.Person
        {
            Name = cmd.Request.Name,
            Age = cmd.Request.Age,
            PersonTypeId = cmd.Request.PersonTypeId
        };
        _db.Persons.Add(person);
        await _db.SaveChangesAsync(ct);
        
        return new PersonResponse(
            person.PersonId,
            person.Name,
            person.Age,
            person.PersonTypeId,
            null
        );
    }
}
