using Person.Contracts;
using Person.Shared.CQRS;

public record GetAllPersonsQuery() : IRequest<List<PersonResponse>>;
