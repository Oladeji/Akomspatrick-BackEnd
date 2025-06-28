using Person.Contracts;
using Person.Shared.CQRS;

public record UpdatePersonCommand(int Id, UpdatePersonRequest Request) : IRequest<int>;
