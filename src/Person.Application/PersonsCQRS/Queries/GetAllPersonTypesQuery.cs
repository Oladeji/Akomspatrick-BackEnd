using Person.Contracts;
using Person.Shared.CQRS;

public record GetAllPersonTypesQuery() : IRequest<List<PersonTypeResponse>>;
