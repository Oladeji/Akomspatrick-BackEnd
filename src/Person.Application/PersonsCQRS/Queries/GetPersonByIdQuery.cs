using Person.Contracts;
using Person.Shared.CQRS;

public record GetPersonByIdQuery(int Id) : IRequest<PersonResponse?>;
