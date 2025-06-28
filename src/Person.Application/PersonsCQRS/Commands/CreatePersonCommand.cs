using Person.Contracts;
using Person.Shared.CQRS;

public record CreatePersonCommand(CreatePersonRequest Request) : IRequest<PersonResponse>;
