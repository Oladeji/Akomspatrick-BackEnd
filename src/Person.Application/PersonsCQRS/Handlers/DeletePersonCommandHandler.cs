using Microsoft.Extensions.Logging;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, int>
{
    private readonly PersonDbContext _db;
    private readonly ILogger<DeletePersonCommandHandler> _logger;

    public DeletePersonCommandHandler(PersonDbContext db, ILogger<DeletePersonCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<int> Handle(DeletePersonCommand cmd, CancellationToken ct)
    {
        _logger.LogInformation("Handling DeletePersonCommand for PersonId: {PersonId}", cmd.Id);

        var person = await _db.Persons.FindAsync(new object[] { cmd.Id }, ct);
        if (person is null)
        {
            _logger.LogWarning("Person not found for deletion. PersonId: {PersonId}", cmd.Id);
            throw new KeyNotFoundException($"Person with Id {cmd.Id} not found.");
        }

        _db.Persons.Remove(person);
        _logger.LogInformation("Person removed from context. PersonId: {PersonId}, Name: {Name}, Age: {Age}, PersonTypeId: {PersonTypeId}",
            person.PersonId, person.Name, person.Age, person.PersonTypeId);

        var result = await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Delete operation completed. Rows affected: {RowsAffected}", result);

        return result;
    }
}
