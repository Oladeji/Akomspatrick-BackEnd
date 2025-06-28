using Person.Shared.CQRS;

public record DeletePersonCommand(int Id) : IRequest<int>;
