using Microsoft.Extensions.Logging;
using Person.Contracts;
using Person.Infrastructure;
using Person.Shared.CQRS;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, int>
{
    private readonly PersonDbContext _db;
    private readonly ILogger<UpdatePersonCommandHandler> _logger;

    public UpdatePersonCommandHandler(PersonDbContext db, ILogger<UpdatePersonCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<int> Handle(UpdatePersonCommand cmd, CancellationToken ct)
    {
        _logger.LogInformation("Handling UpdatePersonCommand for PersonId: {PersonId} with data: {@Request}", cmd.Id, cmd.Request);

        var person = await _db.Persons.FindAsync(new object[] { cmd.Id }, ct);
        if (person is null)
        {
            _logger.LogWarning("Person not found for update. PersonId: {PersonId}", cmd.Id);
            throw new KeyNotFoundException();
        }

        person.Name = cmd.Request.Name;
        person.Age = cmd.Request.Age;
        person.PersonTypeId = cmd.Request.PersonTypeId;

        _logger.LogInformation("Updating person: {@Person}", person);

        var result = await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Update operation completed for PersonId: {PersonId}. Rows affected: {RowsAffected}", person.PersonId, result);

        return result;
    }
}
